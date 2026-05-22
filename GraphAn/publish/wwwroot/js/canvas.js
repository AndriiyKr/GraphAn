document.addEventListener("DOMContentLoaded", () => {
    class GraphEditor {
        constructor() {
            this.svg = document.getElementById('graph-canvas');
            this.zoomGroup = document.getElementById('zoom-group');
            this.mainGroup = document.getElementById('main-group');
            this.mode = 'select';

            this.nodes = [];
            this.edges = [];
            this.nodeCounter = 1;
            this.isDirectedGraph = null;

            this.scale = 1;

            // Стани перетягування вершин
            this.selectedNodeId = null;
            this.isDragging = false;
            this.draggedNodeId = null;

            // Стани перетягування (згинання) ребер
            this.isDraggingEdge = false;
            this.draggedEdgeId = null;

            this.drawingEdgeFrom = null;
            this.tempLine = null;

            this.modals = {
                weight: new bootstrap.Modal(document.getElementById('modalWeight')),
                rename: new bootstrap.Modal(document.getElementById('modalRename'))
            };
            this.tempData = { targetObj: null };

            this.initEvents();
            this.initToolbar();
            this.render();
        }

        initEvents() {
            this.svg.addEventListener('mousedown', this.handleMouseDown.bind(this));
            this.svg.addEventListener('mousemove', this.handleMouseMove.bind(this));
            this.svg.addEventListener('mouseup', this.handleMouseUp.bind(this));
            this.svg.addEventListener('mouseleave', this.handleMouseUp.bind(this));

            this.svg.addEventListener('wheel', (e) => {
                e.preventDefault();
                const scaleAmount = 0.05;
                if (e.deltaY < 0) this.scale += scaleAmount;
                else this.scale -= scaleAmount;
                this.scale = Math.max(0.3, Math.min(this.scale, 3));
                this.zoomGroup.setAttribute('transform', `scale(${this.scale})`);
            }, { passive: false });

            document.getElementById('save-rename').addEventListener('click', () => {
                const val = document.getElementById('input-rename').value;
                if (val && this.tempData.targetObj) {
                    this.tempData.targetObj.label = val;
                    this.render();
                    this.modals.rename.hide();
                }
            });

            document.getElementById('save-weight').addEventListener('click', () => {
                const val = document.getElementById('input-weight').value;
                if (val !== '' && !isNaN(val) && this.tempData.targetObj) {
                    this.tempData.targetObj.weight = parseFloat(val);
                    this.render();
                    this.modals.weight.hide();
                }
            });
        }

        initToolbar() {
            const modes = {
                'btn-select': 'select',
                'btn-add-node': 'addNode',
                'btn-add-edge': 'addEdge',
                'btn-add-dir-edge': 'addDirEdge',
                'btn-add-loop': 'addLoop',
                'btn-rename': 'rename',
                'btn-weight': 'weight',
                'btn-delete': 'delete'
            };

            for (const [btnId, mode] of Object.entries(modes)) {
                const btn = document.getElementById(btnId);
                if (btn) {
                    btn.addEventListener('click', (e) => {
                        if (btn.disabled) return;
                        document.querySelectorAll('.t-btn:not(.text-danger)').forEach(b => b.classList.remove('active'));
                        e.currentTarget.classList.add('active');
                        this.setMode(mode);
                    });
                }
            }

            document.getElementById('btn-clear').addEventListener('click', () => {
                this.nodes = [];
                this.edges = [];
                this.nodeCounter = 1;
                this.isDirectedGraph = null;
                this.updateToolbarStates();
                this.render();
            });
        }

        setMode(mode) {
            this.mode = mode;
            this.selectedNodeId = null;
            this.cancelEdgeDrawing();
            this.render();
        }

        getMouseCoords(e) {
            const rect = this.svg.getBoundingClientRect();
            return {
                x: (e.clientX - rect.left) / this.scale,
                y: (e.clientY - rect.top) / this.scale
            };
        }

        handleMouseDown(e) {
            const { x, y } = this.getMouseCoords(e);
            const targetG = e.target.closest('.node-group');
            const clickedNodeId = targetG ? parseInt(targetG.getAttribute('data-id')) : null;

            switch (this.mode) {
                case 'addNode':
                    if (!clickedNodeId) {
                        this.nodes.push({ id: Date.now(), label: `v${this.nodeCounter++}`, x, y });
                        this.render();
                    }
                    break;
                case 'select':
                    if (clickedNodeId) {
                        // Перетягування вершини
                        this.selectedNodeId = clickedNodeId;
                        this.isDragging = true;
                        this.draggedNodeId = clickedNodeId;
                        this.render();
                    } else {
                        // Перетягування (вигинання) ребра
                        const targetEdge = e.target.closest('.edge-weight-group');
                        if (targetEdge) {
                            const edgeId = parseInt(targetEdge.getAttribute('data-id'));
                            const edge = this.edges.find(e => e.id === edgeId);
                            // Згинати можна тільки звичайні ребра (не петлі)
                            if (edge && edge.source !== edge.target) {
                                this.isDraggingEdge = true;
                                this.draggedEdgeId = edgeId;
                            }
                        }
                        this.selectedNodeId = null;
                        this.render();
                    }
                    break;
                case 'addEdge':
                case 'addDirEdge':
                    if (clickedNodeId) {
                        this.drawingEdgeFrom = clickedNodeId;
                        const startNode = this.nodes.find(n => n.id === clickedNodeId);
                        this.tempLine = document.createElementNS("http://www.w3.org/2000/svg", "line");
                        this.tempLine.setAttribute("stroke", "#adb5bd");
                        this.tempLine.setAttribute("stroke-width", "2");
                        this.tempLine.setAttribute("stroke-dasharray", "5,5");
                        this.tempLine.setAttribute("x1", startNode.x);
                        this.tempLine.setAttribute("y1", startNode.y);
                        this.tempLine.setAttribute("x2", x);
                        this.tempLine.setAttribute("y2", y);
                        this.mainGroup.appendChild(this.tempLine);
                    }
                    break;
                case 'addLoop':
                    if (clickedNodeId) {
                        const directed = this.isDirectedGraph !== null ? this.isDirectedGraph : false;
                        if (this.isDirectedGraph === null) this.isDirectedGraph = directed;
                        this.edges.push({ id: Date.now(), source: clickedNodeId, target: clickedNodeId, weight: null, directed, bend: 0 });
                        this.updateToolbarStates();
                        this.render();
                    }
                    break;
                case 'rename':
                    if (clickedNodeId) {
                        this.tempData.targetObj = this.nodes.find(n => n.id === clickedNodeId);
                        document.getElementById('input-rename').value = this.tempData.targetObj.label;
                        this.modals.rename.show();
                    }
                    break;
                case 'weight':
                    const tw = e.target.closest('.edge-weight-group');
                    if (tw) {
                        const edgeId = parseInt(tw.getAttribute('data-id'));
                        this.tempData.targetObj = this.edges.find(edge => edge.id === edgeId);
                        if (this.tempData.targetObj) {
                            document.getElementById('input-weight').value = this.tempData.targetObj.weight !== null ? this.tempData.targetObj.weight : '';
                            this.modals.weight.show();
                        }
                    }
                    break;
                case 'delete':
                    if (clickedNodeId) {
                        this.nodes = this.nodes.filter(n => n.id !== clickedNodeId);
                        this.edges = this.edges.filter(edge => edge.source !== clickedNodeId && edge.target !== clickedNodeId);
                    } else {
                        const targetEdgeDel = e.target.closest('.edge-weight-group');
                        if (targetEdgeDel) {
                            const edgeId = parseInt(targetEdgeDel.getAttribute('data-id'));
                            this.edges = this.edges.filter(edge => edge.id !== edgeId);
                        }
                    }
                    this.updateToolbarStates();
                    this.render();
                    break;
            }
        }

        handleMouseMove(e) {
            const { x, y } = this.getMouseCoords(e);

            // Якщо тягнемо вузол
            if (this.isDragging && this.draggedNodeId) {
                const node = this.nodes.find(n => n.id === this.draggedNodeId);
                if (node) {
                    node.x = x;
                    node.y = y;
                    this.render();
                }
            }

            // Якщо тягнемо/вигинаємо ребро
            if (this.isDraggingEdge && this.draggedEdgeId) {
                const edge = this.edges.find(e => e.id === this.draggedEdgeId);
                if (edge) {
                    const s = this.nodes.find(n => n.id === edge.source);
                    const t = this.nodes.find(n => n.id === edge.target);

                    const mx = (s.x + t.x) / 2;
                    const my = (s.y + t.y) / 2;
                    const dx = t.x - s.x;
                    const dy = t.y - s.y;
                    const len = Math.sqrt(dx * dx + dy * dy);

                    if (len > 0) {
                        // Перпендикулярний нормальний вектор
                        const nx = -dy / len;
                        const ny = dx / len;

                        // Відстань від центру ребра до курсора миші
                        const cx = 2 * x - mx;
                        const cy = 2 * y - my;

                        // Розрахунок сили вигину
                        edge.bend = (cx - mx) * nx + (cy - my) * ny;
                        this.render();
                    }
                }
            }

            // Протягування пунктирної лінії при додаванні
            if ((this.mode === 'addEdge' || this.mode === 'addDirEdge') && this.drawingEdgeFrom && this.tempLine) {
                this.tempLine.setAttribute("x2", x);
                this.tempLine.setAttribute("y2", y);
            }
        }

        handleMouseUp(e) {
            this.isDragging = false;
            this.draggedNodeId = null;
            this.isDraggingEdge = false;
            this.draggedEdgeId = null;

            if ((this.mode === 'addEdge' || this.mode === 'addDirEdge') && this.drawingEdgeFrom) {
                const targetG = e.target.closest('.node-group');
                const targetNodeId = targetG ? parseInt(targetG.getAttribute('data-id')) : null;

                if (targetNodeId && targetNodeId !== this.drawingEdgeFrom) {
                    const directed = this.mode === 'addDirEdge';
                    if (this.isDirectedGraph === null) this.isDirectedGraph = directed;

                    // Рахуємо вже існуючі ребра між цими вершинами, щоб зробити вигин автоматичним
                    const relatedEdges = this.edges.filter(edge =>
                        (edge.source === this.drawingEdgeFrom && edge.target === targetNodeId) ||
                        (edge.source === targetNodeId && edge.target === this.drawingEdgeFrom)
                    );

                    let newBend = 0;
                    if (relatedEdges.length > 0) {
                        const count = relatedEdges.length;
                        const sign = count % 2 === 0 ? 1 : -1;
                        newBend = Math.ceil(count / 2) * 40 * sign; // Вигин: 40, -40, 80, -80...
                    }

                    this.edges.push({
                        id: Date.now(),
                        source: this.drawingEdgeFrom,
                        target: targetNodeId,
                        weight: null,
                        directed: this.isDirectedGraph,
                        bend: newBend
                    });
                    this.updateToolbarStates();
                }
                this.cancelEdgeDrawing();
                this.render();
            }
        }

        cancelEdgeDrawing() {
            if (this.tempLine) {
                this.tempLine.remove();
                this.tempLine = null;
            }
            this.drawingEdgeFrom = null;
        }

        updateToolbarStates() {
            const btnEdge = document.getElementById('btn-add-edge');
            const btnDirEdge = document.getElementById('btn-add-dir-edge');

            if (this.edges.length > 0) {
                if (this.isDirectedGraph) {
                    btnEdge.disabled = true;
                    if (this.mode === 'addEdge') document.getElementById('btn-select').click();
                } else {
                    btnDirEdge.disabled = true;
                    if (this.mode === 'addDirEdge') document.getElementById('btn-select').click();
                }
            } else {
                btnEdge.disabled = false;
                btnDirEdge.disabled = false;
                this.isDirectedGraph = null;
            }
        }

        render() {
            Array.from(this.mainGroup.children).forEach(el => {
                if (el !== this.tempLine) el.remove();
            });

            this.edges.forEach(edge => {
                const s = this.nodes.find(n => n.id === edge.source);
                const t = this.nodes.find(n => n.id === edge.target);
                if (!s || !t) return;

                const group = document.createElementNS("http://www.w3.org/2000/svg", "g");
                group.setAttribute("class", "edge-weight-group");
                group.setAttribute("data-id", edge.id);

                if (s.id === t.id) {
                    // ===== МАЛЮВАННЯ ПЕТЛІ =====
                    const loops = this.edges.filter(e => e.source === s.id && e.target === s.id);
                    const idx = loops.findIndex(e => e.id === edge.id);

                    // Збільшуємо висоту та ширину петлі для кратних петель
                    const h = 80 + idx * 40;
                    const w = 40 + idx * 25;

                    const d = `M ${s.x} ${s.y - 20} C ${s.x - w} ${s.y - h}, ${s.x + w} ${s.y - h}, ${s.x} ${s.y - 20}`;

                    const path = document.createElementNS("http://www.w3.org/2000/svg", "path");
                    path.setAttribute("d", d);
                    path.setAttribute("class", "graph-edge");
                    if (edge.directed) path.setAttribute("marker-end", "url(#arrowhead)");

                    const hitPath = document.createElementNS("http://www.w3.org/2000/svg", "path");
                    hitPath.setAttribute("d", d);
                    hitPath.setAttribute("class", "edge-hit-area");

                    group.appendChild(path);
                    group.appendChild(hitPath);

                    if (edge.weight !== null) {
                        const text = document.createElementNS("http://www.w3.org/2000/svg", "text");
                        text.setAttribute("x", s.x);
                        text.setAttribute("y", s.y - h - 5);
                        text.setAttribute("text-anchor", "middle");
                        text.setAttribute("class", "edge-weight-text");
                        text.textContent = edge.weight;
                        group.appendChild(text);
                    }
                } else {
                    // ===== МАЛЮВАННЯ РЕБРА З ВИГИНОМ (Крива Безьє) =====
                    const mx = (s.x + t.x) / 2;
                    const my = (s.y + t.y) / 2;
                    const dx = t.x - s.x;
                    const dy = t.y - s.y;
                    const len = Math.sqrt(dx * dx + dy * dy);

                    const nx = len > 0 ? -dy / len : 0;
                    const ny = len > 0 ? dx / len : 0;

                    const bend = edge.bend || 0;
                    const cx = mx + bend * nx;
                    const cy = my + bend * ny;

                    const d = `M ${s.x} ${s.y} Q ${cx} ${cy} ${t.x} ${t.y}`;

                    const path = document.createElementNS("http://www.w3.org/2000/svg", "path");
                    path.setAttribute("d", d);
                    path.setAttribute("class", "graph-edge");
                    if (edge.directed) path.setAttribute("marker-end", "url(#arrowhead)");

                    const hitPath = document.createElementNS("http://www.w3.org/2000/svg", "path");
                    hitPath.setAttribute("d", d);
                    hitPath.setAttribute("class", "edge-hit-area");

                    group.appendChild(path);
                    group.appendChild(hitPath);

                    if (edge.weight !== null) {
                        // Точка центру кривої Безьє (t=0.5)
                        const px = 0.25 * s.x + 0.5 * cx + 0.25 * t.x;
                        const py = 0.25 * s.y + 0.5 * cy + 0.25 * t.y;

                        // Зміщуємо текст відносно центру кривої
                        const textOffset = bend >= 0 ? 15 : -15;
                        const textX = px + textOffset * nx;
                        const textY = py + textOffset * ny;

                        const text = document.createElementNS("http://www.w3.org/2000/svg", "text");
                        text.setAttribute("x", textX);
                        text.setAttribute("y", textY);
                        text.setAttribute("text-anchor", "middle");
                        text.setAttribute("dominant-baseline", "central");
                        text.setAttribute("class", "edge-weight-text");
                        text.textContent = edge.weight;
                        group.appendChild(text);
                    }
                }

                this.mainGroup.appendChild(group);
            });

            this.nodes.forEach(node => {
                const g = document.createElementNS("http://www.w3.org/2000/svg", "g");
                g.setAttribute("class", `node-group ${this.selectedNodeId === node.id ? 'selected' : ''}`);
                g.setAttribute("data-id", node.id);
                g.innerHTML = `
                    <circle class="graph-node" cx="${node.x}" cy="${node.y}" r="22"></circle>
                    <text class="node-label" x="${node.x}" y="${node.y}" text-anchor="middle" dominant-baseline="central">${node.label}</text>
                `;
                this.mainGroup.appendChild(g);
            });
        }
    }

    window.graphEditor = new GraphEditor();
});