// History Management (Undo/Redo)
class HistoryManager {
    constructor(limit = 50) {
        this.history = [];
        this.redoStack = [];
        this.limit = limit;
    }

    push(state) {
        if (this.history.length > 0 && JSON.stringify(this.history[this.history.length - 1]) === JSON.stringify(state)) return;
        this.history.push(JSON.parse(JSON.stringify(state)));
        if (this.history.length > this.limit) this.history.shift();
        this.redoStack = [];
        this.updateButtons();
    }

    undo() {
        if (this.history.length <= 1) return null;
        this.redoStack.push(this.history.pop());
        this.updateButtons();
        return JSON.parse(JSON.stringify(this.history[this.history.length - 1]));
    }

    redo() {
        if (this.redoStack.length === 0) return null;
        const state = this.redoStack.pop();
        this.history.push(state);
        this.updateButtons();
        return JSON.parse(JSON.stringify(state));
    }

    updateButtons() {
        const undoBtn = document.querySelector('[title="Undo"]');
        const redoBtn = document.querySelector('[title="Redo"]');
        if (undoBtn) undoBtn.classList.toggle('disabled', this.history.length <= 1);
        if (redoBtn) redoBtn.classList.toggle('disabled', this.redoStack.length === 0);
    }
}

const history = new HistoryManager();

// Screen navigation — maps to MVC URLs
function go(n) {
    if (n === 5) {
        window.location.href = '/Templates';
        return;
    }
    const target = document.getElementById('s' + n);
    if (!target) {
        window.location.href = '/TemplateBuilder/Editor';
        return;
    }
    document.querySelectorAll('.screen').forEach(s => s.classList.remove('show'));
    target.classList.add('show');
    
    // Sync buttons in topbar
    document.querySelectorAll('.mode-btn').forEach((btn, idx) => {
        btn.classList.toggle('on', idx + 1 === n);
    });
}

// Sidebar tab switching
function sbTab(el, id) {
    document.querySelectorAll('.sb-tab').forEach(t => t.classList.remove('on'));
    el.classList.add('on');
    ['t1', 't2', 't3', 't4'].forEach(t => {
        const p = document.getElementById(t);
        if (p) p.style.display = t === id ? 'block' : 'none';
    });
}

// Right panel tab switching
function rpTab(el, id) {
    document.querySelectorAll('.rp-tab').forEach(t => t.classList.remove('on'));
    el.classList.add('on');
    ['rp1', 'rp2', 'rp3'].forEach(t => {
        const p = document.getElementById(t);
        if (p) p.style.display = t === id ? 'block' : 'none';
    });
}

// Grid toggle on canvas
let gridOn = true;
function toggleGrid() {
    gridOn = !gridOn;
    const cw = document.getElementById('canvas-wrap');
    const gb = document.getElementById('grid-btn');
    if (cw) {
        cw.style.backgroundImage = gridOn
            ? 'radial-gradient(circle, rgba(0,0,0,0.12) 1px, transparent 1px)'
            : 'none';
        cw.style.backgroundSize = '16px 16px';
    }
    if (gb) gb.classList.toggle('on', gridOn);
}

// Canvas element selection and Dragging
function initCanvasSelection() {
    document.querySelectorAll('.cel').forEach(el => {
        el.addEventListener('mousedown', function (e) {
            if (e.target.classList.contains('rh')) return; // let resizer handle it
            e.stopPropagation();
            selectElement(this);
            startDragging(this, e);
        });
    });
    const paper = document.getElementById('canvas-paper');
    if (paper) {
        paper.addEventListener('mousedown', function (e) {
            if (e.target === this) clearSelection();
        });
    }
}

function selectElement(el) {
    document.querySelectorAll('.cel').forEach(c => c.classList.remove('sel'));
    el.classList.add('sel');
    updatePropertyPanel(el);
}

function clearSelection() {
    document.querySelectorAll('.cel').forEach(c => c.classList.remove('sel'));
}

function updatePropertyPanel(el) {
    // Sync element properties to the right panel
    const x = parseInt(el.style.left) || 0;
    const y = parseInt(el.style.top) || 0;
    const w = parseInt(el.style.width) || el.offsetWidth;
    const h = parseInt(el.style.height) || el.offsetHeight;
    
    const inputs = document.querySelectorAll('.pf-inp');
    if (inputs.length >= 4) {
        inputs[0].value = x;
        inputs[1].value = y;
        inputs[2].value = w;
        inputs[3].value = h;
    }
}

// Dragging Logic
function startDragging(el, e) {
    const parent = el.offsetParent;
    const startX = e.clientX;
    const startY = e.clientY;
    const startLeft = parseInt(el.style.left) || 0;
    const startTop = parseInt(el.style.top) || 0;

    function onMove(me) {
        let dx = me.clientX - startX;
        let dy = me.clientY - startY;
        
        let newLeft = startLeft + dx;
        let newTop = startTop + dy;

        // Snap to grid (16px)
        if (gridOn) {
            newLeft = Math.round(newLeft / 16) * 16;
            newTop = Math.round(newTop / 16) * 16;
        }

        el.style.left = newLeft + 'px';
        el.style.top = newTop + 'px';
        updatePropertyPanel(el);
    }

    function onUp() {
        document.removeEventListener('mousemove', onMove);
        document.removeEventListener('mouseup', onUp);
        saveState();
    }

    document.addEventListener('mousemove', onMove);
    document.addEventListener('mouseup', onUp);
}

// Zoom slider
function initZoomSlider() {
    const zmSlider = document.getElementById('zm-slider');
    const zmVal = document.getElementById('zm-val');
    const paper = document.getElementById('canvas-paper');
    if (zmSlider && zmVal) {
        zmSlider.addEventListener('input', function () {
            const val = this.value;
            zmVal.textContent = val + '%';
            if (paper) paper.style.transform = `scale(${val/100})`;
        });
    }
}

// Add line item row in Data screen
function addItem() {
    const tbody = document.getElementById('items-tbody');
    if (!tbody) return;
    const tr = document.createElement('tr');
    tr.className = 'item-row';
    
    // Fetch available taxes from a hidden source or the first row's picker
    const firstPicker = document.querySelector('.tp-dropdown');
    let taxOptions = '';
    if (firstPicker) {
        taxOptions = firstPicker.innerHTML;
    }

    tr.innerHTML = `
        <td><input class="item-desc" value="New Item" oninput="calculateTotals()"></td>
        <td><input class="item-qty" type="number" value="1" oninput="calculateTotals()"></td>
        <td><input class="item-price" value="0.00" oninput="calculateTotals()"></td>
        <td>
            <div class="tax-picker">
                <div class="tp-trigger" onclick="toggleTP(this)">Select taxes...</div>
                <div class="tp-dropdown">${taxOptions}</div>
            </div>
        </td>
        <td class="item-amount" style="font-weight:600;font-size:12px">$0.00</td>
        <td>
            <button class="rm-btn" onclick="this.closest('tr').remove(); calculateTotals();">
                <svg viewBox="0 0 11 11" fill="none" stroke="currentColor" stroke-width="1.4" width="11" height="11">
                    <line x1="2" y1="2" x2="9" y2="9"/><line x1="9" y1="2" x2="2" y2="9"/>
                </svg>
            </button>
        </td>`;
    tbody.appendChild(tr);
    // Reset checkboxes in the new row
    tr.querySelectorAll('input[type="checkbox"]').forEach(cb => cb.checked = false);
    calculateTotals();
    saveState();
}

function toggleTP(el) {
    const dropdown = el.nextElementSibling;
    const isShow = dropdown.classList.contains('show');
    
    // Close other dropdowns
    document.querySelectorAll('.tp-dropdown').forEach(d => d.classList.remove('show'));
    
    if (!isShow) dropdown.classList.add('show');
}

// Close dropdowns on outside click
document.addEventListener('click', (e) => {
    if (!e.target.closest('.tax-picker')) {
        document.querySelectorAll('.tp-dropdown').forEach(d => d.classList.remove('show'));
    }
});

function onTaxUpdate(el) {
    const picker = el.closest('.tax-picker');
    const trigger = picker.querySelector('.tp-trigger');
    const checked = picker.querySelectorAll('input:checked');
    
    if (checked.length === 0) {
        trigger.textContent = "Select taxes...";
    } else if (checked.length === 1) {
        trigger.textContent = checked[0].getAttribute('data-name');
    } else {
        trigger.textContent = `${checked.length} taxes selected`;
    }
    
    calculateTotals();
}

function calculateTotals() {
    let subtotal = 0;
    const taxesAggr = {}; // { name: { amount, rate } }

    document.querySelectorAll('.item-row').forEach(row => {
        const qty = parseFloat(row.querySelector('.item-qty').value) || 0;
        let price = row.querySelector('.item-price').value.replace(/[^0-9.-]+/g, "");
        price = parseFloat(price) || 0;
        
        const lineAmount = qty * price;
        subtotal += lineAmount;

        // Update item amount cell
        row.querySelector('.item-amount').textContent = '$' + lineAmount.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });

        // Taxes from checkboxes
        const checkboxes = Array.from(row.querySelectorAll('.tp-opt input:checked'));
        
        // Sort by priority
        checkboxes.sort((a, b) => {
            const pA = parseInt(a.getAttribute('data-priority')) || 0;
            const pB = parseInt(b.getAttribute('data-priority')) || 0;
            return pA - pB;
        });

        let lineTaxesTotal = 0;

        checkboxes.forEach(cb => {
            const name = cb.getAttribute('data-name');
            const rate = parseFloat(cb.getAttribute('data-rate')) || 0;
            const isCompound = cb.getAttribute('data-compound') === 'true';
            
            const base = isCompound ? (lineAmount + lineTaxesTotal) : lineAmount;
            const taxAmount = base * (rate / 100);
            lineTaxesTotal += taxAmount;

            if (!taxesAggr[name]) {
                taxesAggr[name] = { amount: 0, rate: rate };
            }
            taxesAggr[name].amount += taxAmount;
        });
    });

    const totalTax = Object.values(taxesAggr).reduce((s, t) => s + t.amount, 0);
    const total = subtotal + totalTax;

    // Update Totals Summary
    document.querySelectorAll('[data-bind-target="Subtotal"]').forEach(el => el.textContent = subtotal.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }));
    document.querySelectorAll('[data-bind-target="Total"]').forEach(el => el.textContent = total.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }));

    const container = document.getElementById('taxes-container');
    const pContainer = document.getElementById('p-taxes-container');
    if (container) container.innerHTML = '';
    if (pContainer) pContainer.innerHTML = '';

    for (const [name, data] of Object.entries(taxesAggr)) {
        const amountStr = `$${data.amount.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
        
        if (container) {
            const div = document.createElement('div');
            div.className = 'tm-row tax-row';
            div.innerHTML = `<span>${name} (${data.rate}%)</span><span>${amountStr}</span>`;
            container.appendChild(div);
        }
        if (pContainer) {
            const div = document.createElement('div');
            div.className = 'p-tot-row tax-row';
            div.innerHTML = `<span>${name} (${data.rate}%)</span><span>${amountStr}</span>`;
            pContainer.appendChild(div);
        }
    }

    // Update Design/Preview (Simple sync for now)
    document.querySelectorAll('[data-bind-target="Tax"]').forEach(el => {
        el.textContent = totalTax.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    });
}

// State management
function getCurrentState() {
    const elements = [];
    document.querySelectorAll('.cel').forEach(el => {
        elements.push({
            id: el.getAttribute('data-lbl'),
            left: el.style.left,
            top: el.style.top,
            width: el.style.width,
            height: el.style.height,
            content: el.innerHTML
        });
    });
    const dataInputs = {};
    document.querySelectorAll('[data-bind-src]').forEach(inp => {
        dataInputs[inp.getAttribute('data-bind-src')] = inp.value;
    });

    const lineItems = [];
    document.querySelectorAll('.item-row').forEach(row => {
        lineItems.push({
            description: row.querySelector('.item-desc').value,
            qty: row.querySelector('.item-qty').value,
            price: row.querySelector('.item-price').value,
            taxIds: Array.from(row.querySelectorAll('.tp-opt input:checked')).map(cb => cb.value)
        });
    });

    return { elements, dataInputs, lineItems };
}

function restoreState(state) {
    if (!state) return;
    state.elements.forEach(saved => {
        const el = document.querySelector(`.cel[data-lbl="${saved.id}"]`);
        if (el) {
            el.style.left = saved.left;
            el.style.top = saved.top;
            el.style.width = saved.width;
            el.style.height = saved.height;
        }
    });
    for (const [key, val] of Object.entries(state.dataInputs)) {
        const inp = document.querySelector(`[data-bind-src="${key}"]`);
        if (inp) {
            inp.value = val;
            updateBind(inp);
        }
    }

    if (state.lineItems) {
        const tbody = document.getElementById('items-tbody');
        if (tbody) {
            tbody.innerHTML = '';
            state.lineItems.forEach(item => {
                addItem();
                const lastRow = tbody.lastElementChild;
                lastRow.querySelector('.item-desc').value = item.description;
                lastRow.querySelector('.item-qty').value = item.qty;
                lastRow.querySelector('.item-price').value = item.price;
                const dropdown = lastRow.querySelector('.tp-dropdown');
                item.taxIds.forEach(id => {
                    const cb = dropdown.querySelector(`input[value="${id}"]`);
                    if (cb) cb.checked = true;
                });
                onTaxUpdate(lastRow.querySelector('input[type="checkbox"]'));
            });
            calculateTotals();
        }
    }
}

function saveState() {
    const state = getCurrentState();
    history.push(state);
    debouncedAutoSave();
}

// Undo/Redo actions
function undo() {
    const state = history.undo();
    if (state) restoreState(state);
}

function redo() {
    const state = history.redo();
    if (state) restoreState(state);
}

// Auto-save logic
let autoSaveTimeout;
function debouncedAutoSave() {
    clearTimeout(autoSaveTimeout);
    autoSaveTimeout = setTimeout(() => {
        saveTemplate(true);
    }, 5000); // 5 second debounce
}

// Toggle switch
document.addEventListener('click', function (e) {
    if (e.target.classList.contains('tgl')) {
        e.target.classList.toggle('on');
    }
});

// Data Binding Logic
function updateBind(inp) {
    const key = inp.getAttribute('data-bind-src');
    if (!key) return;
    document.querySelectorAll(`[data-bind-target="${key}"]`).forEach(el => {
        el.textContent = inp.value;
    });
}

function initBindings() {
    document.querySelectorAll('[data-bind-src]').forEach(inp => {
        inp.addEventListener('input', () => {
            updateBind(inp);
            saveState();
        });
        updateBind(inp); 
    });
}

function saveTemplate(isAuto = false) {
    const name = document.getElementById('tpl-name-inp')?.value || 'New Template';
    const payload = {
        name: name,
        templateJson: JSON.stringify(getCurrentState()),
        isDefault: false
    };
    
    const saveBtn = document.querySelector('.btn-primary');
    const originalText = saveBtn ? saveBtn.textContent : '';
    if (saveBtn && !isAuto) saveBtn.textContent = 'Saving...';

    fetch('/TemplateBuilder/Save', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(payload)
    })
    .then(r => r.json())
    .then(data => {
        if (saveBtn && !isAuto) saveBtn.textContent = originalText;
        if(data.success) {
            if (!isAuto) alert(data.message);
            console.log('Saved:', data.templateId);
        } else {
            console.error('Save failed:', data.message);
        }
    })
    .catch(e => {
        if (saveBtn && !isAuto) saveBtn.textContent = originalText;
        console.error(e);
    });
}

// Init on DOM ready
document.addEventListener('DOMContentLoaded', function () {
    initCanvasSelection();
    initZoomSlider();
    initBindings();
    initComponentDragDrop();
    
    // Initial state
    history.push(getCurrentState());

    // Hook up undo/redo buttons
    const undoBtn = document.querySelector('[title="Undo"]');
    const redoBtn = document.querySelector('[title="Redo"]');
    if (undoBtn) undoBtn.onclick = undo;
    if (redoBtn) redoBtn.onclick = redo;
});

function initComponentDragDrop() {
    const components = document.querySelectorAll('.comp-row');
    const paper = document.getElementById('canvas-paper');

    components.forEach(comp => {
        comp.addEventListener('dragstart', (e) => {
            const type = comp.querySelector('.comp-title').textContent;
            e.dataTransfer.setData('text/plain', type);
        });
    });

    if (paper) {
        paper.addEventListener('dragover', (e) => {
            e.preventDefault();
        });

        paper.addEventListener('drop', (e) => {
            e.preventDefault();
            const type = e.dataTransfer.getData('text/plain');
            const rect = paper.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;
            
            addNewComponent(type, x, y);
        });
    }
}

function addNewComponent(type, x, y) {
    const paper = document.getElementById('canvas-paper');
    const id = type + '_' + Date.now();
    const div = document.createElement('div');
    div.className = 'cel';
    div.setAttribute('data-lbl', type);
    div.style.left = (Math.round(x / 16) * 16) + 'px';
    div.style.top = (Math.round(y / 16) * 16) + 'px';
    div.style.width = '150px';
    div.style.height = '40px';
    
    div.innerHTML = `
        <div class="cel-label">${type}</div>
        <div class="rh rh-tl"></div><div class="rh rh-tr"></div><div class="rh rh-bl"></div><div class="rh rh-br"></div>
        <div style="padding:5px; border:1px dashed #ccc; font-size:12px; color:#666">${type} Placeholder</div>
    `;

    div.addEventListener('mousedown', function (e) {
        if (e.target.classList.contains('rh')) return;
        e.stopPropagation();
        selectElement(this);
        startDragging(this, e);
    });

    paper.appendChild(div);
    selectElement(div);
    saveState();
}

