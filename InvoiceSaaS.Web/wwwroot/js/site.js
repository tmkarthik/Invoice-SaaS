// Screen navigation — maps to MVC URLs
function go(n) {
    if (n === 5) {
        window.location.href = '/Templates';
        return;
    }
    const target = document.getElementById('s' + n);
    if (!target) {
        // If we are not on the builder page at all, redirect to it.
        window.location.href = '/TemplateBuilder/Editor';
        return;
    }
    document.querySelectorAll('.screen').forEach(s => s.classList.remove('show'));
    target.classList.add('show');
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

// Canvas element selection
function initCanvasSelection() {
    document.querySelectorAll('.cel').forEach(el => {
        el.addEventListener('click', function (e) {
            e.stopPropagation();
            document.querySelectorAll('.cel').forEach(c => c.classList.remove('sel'));
            this.classList.add('sel');
        });
    });
    const paper = document.getElementById('canvas-paper');
    if (paper) {
        paper.addEventListener('click', function (e) {
            if (e.target === this) document.querySelectorAll('.cel').forEach(c => c.classList.remove('sel'));
        });
    }
}

// Zoom slider
function initZoomSlider() {
    const zmSlider = document.getElementById('zm-slider');
    const zmVal = document.getElementById('zm-val');
    if (zmSlider && zmVal) {
        zmSlider.addEventListener('input', function () {
            zmVal.textContent = this.value + '%';
        });
    }
}

// Add line item row in Data screen
function addItem() {
    const tbody = document.getElementById('items-tbody');
    if (!tbody) return;
    const tr = document.createElement('tr');
    tr.innerHTML = `<td><input value="New Item"></td><td><input value="1"></td><td><input value="$0.00"></td><td style="font-weight:600;font-size:12px">$0.00</td><td><button class="rm-btn" onclick="this.closest('tr').remove()"><svg viewBox="0 0 11 11" fill="none" stroke="currentColor" stroke-width="1.4" width="11" height="11"><line x1="2" y1="2" x2="9" y2="9"/><line x1="9" y1="2" x2="2" y2="9"/></svg></button></td>`;
    tbody.appendChild(tr);
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
        updateBind(inp); // trigger initialization
    });
}

function saveTemplate() {
    const name = document.getElementById('tpl-name-inp')?.value || 'New Template';
    // Package up the basic data that you want to save.
    const payload = {
        name: name,
        templateJson: JSON.stringify({ 
           // Future: serialize actual canvas architecture. For now save empty/placeholder.
           companyName: document.querySelector('[data-bind-src="CompanyName"]')?.value
        })
    };
    
    fetch('/TemplateBuilder/Save', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
        },
        body: JSON.stringify(payload)
    })
    .then(r => r.json())
    .then(data => {
        if(data.success) alert(data.message);
        else alert('Error: ' + data.message);
    })
    .catch(e => console.error(e));
}

// Init on DOM ready
document.addEventListener('DOMContentLoaded', function () {
    toggleGrid();
    initCanvasSelection();
    initZoomSlider();
    initBindings();
});
