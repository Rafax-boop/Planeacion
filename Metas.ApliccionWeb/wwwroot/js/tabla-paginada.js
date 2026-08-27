// Componente reutilizable de paginación client-side para tablas de listado.
//
// No pelea con los filtros/orden que ya existen en cada vista: lee
// row.style.display para saber qué filas dejó "vivas" el filtro (o todas si
// no hay filtro), y sobre ESE conjunto aplica la paginación usando una CLASE
// propia (no toca style.display) — así el filtro sigue siendo la única
// fuente de verdad de "esta fila cumple el filtro".
//
// Uso:
//   TablaPaginada.iniciar('tablaDatos', { pageSize: 9 });
//   // ...dentro del filtro/orden existente, al final:
//   TablaPaginada.actualizar('tablaDatos');
window.TablaPaginada = (function () {
    var estado = {};
    var alturaFilaCache = {};
    var CLASE_OCULTA = 'tabla-paginacion-oculta';
    var PRIMERAS = 3;
    var ULTIMAS = 3;

    function filasElegibles(tabla) {
        return Array.prototype.filter.call(tabla.querySelectorAll('tbody tr'), function (fila) {
            return fila.style.display !== 'none';
        });
    }

    // Evita que la tabla "salte" de alto cuando la última página tiene menos
    // filas que las demás: mide el alto real de una fila (una sola vez, la
    // cachea) y fija un min-height equivalente a "pageSize" filas completas.
    function fijarAltura(tabla, tablaId, totalPaginas, pageSize) {
        var wrap = tabla.closest('.tabla-datos-wrap');
        if (!wrap) return;

        if (totalPaginas <= 1) {
            wrap.style.minHeight = '';
            return;
        }

        if (!alturaFilaCache[tablaId]) {
            var filaVisible = Array.prototype.find.call(tabla.querySelectorAll('tbody tr'), function (fila) {
                return !fila.classList.contains(CLASE_OCULTA) && fila.style.display !== 'none';
            });
            if (filaVisible) {
                var alto = filaVisible.getBoundingClientRect().height;
                if (alto > 0) alturaFilaCache[tablaId] = alto;
            }
        }

        if (alturaFilaCache[tablaId]) {
            var thead = tabla.querySelector('thead');
            var altoHeader = thead ? thead.getBoundingClientRect().height : 0;
            wrap.style.minHeight = (altoHeader + alturaFilaCache[tablaId] * pageSize) + 'px';
        }
    }

    function calcularPaginasVisibles(actual, total) {
        var incluidas = {};
        var i;
        for (i = 1; i <= Math.min(PRIMERAS, total); i++) incluidas[i] = true;
        for (i = Math.max(1, total - ULTIMAS + 1); i <= total; i++) incluidas[i] = true;
        for (i = Math.max(1, actual - 1); i <= Math.min(total, actual + 1); i++) incluidas[i] = true;

        var ordenadas = Object.keys(incluidas).map(Number).sort(function (a, b) { return a - b; });
        var resultado = [];
        for (i = 0; i < ordenadas.length; i++) {
            if (i > 0 && ordenadas[i] - ordenadas[i - 1] > 1) resultado.push('…');
            resultado.push(ordenadas[i]);
        }
        return resultado;
    }

    function crearBotonTexto(texto, deshabilitado, onClick) {
        var btn = document.createElement('button');
        btn.type = 'button';
        btn.className = 'tabla-paginacion-btn';
        btn.textContent = texto;
        if (deshabilitado) {
            btn.disabled = true;
        } else {
            btn.addEventListener('click', onClick);
        }
        return btn;
    }

    function render(tablaId) {
        var e = estado[tablaId];
        var tabla = document.getElementById(tablaId);
        var contenedor = document.getElementById(tablaId + '-paginacion');
        if (!e || !tabla || !contenedor) return;

        var elegibles = filasElegibles(tabla);
        var totalPaginas = Math.max(1, Math.ceil(elegibles.length / e.pageSize));
        if (e.pagina > totalPaginas) e.pagina = totalPaginas;
        if (e.pagina < 1) e.pagina = 1;

        var inicio = (e.pagina - 1) * e.pageSize;
        var fin = inicio + e.pageSize;

        elegibles.forEach(function (fila, i) {
            fila.classList.toggle(CLASE_OCULTA, !(i >= inicio && i < fin));
        });

        fijarAltura(tabla, tablaId, totalPaginas, e.pageSize);

        contenedor.innerHTML = '';
        if (elegibles.length === 0) return;

        var info = document.createElement('div');
        info.className = 'tabla-paginacion-info';
        info.textContent = 'Mostrando ' + (inicio + 1) + '-' + Math.min(fin, elegibles.length) + ' de ' + elegibles.length + ' registros';
        contenedor.appendChild(info);

        var botones = document.createElement('div');
        botones.className = 'tabla-paginacion-btns';

        botones.appendChild(crearBotonTexto('Anterior', e.pagina <= 1, function () {
            estado[tablaId].pagina--;
            render(tablaId);
        }));

        var nums = document.createElement('span');
        nums.className = 'tabla-paginacion-nums';
        calcularPaginasVisibles(e.pagina, totalPaginas).forEach(function (item) {
            if (item === '…') {
                var puntos = document.createElement('span');
                puntos.className = 'tabla-paginacion-ellipsis';
                puntos.textContent = '…';
                nums.appendChild(puntos);
                return;
            }
            var btn = document.createElement('button');
            btn.type = 'button';
            btn.className = 'tabla-paginacion-btn tabla-paginacion-num' + (item === e.pagina ? ' activo' : '');
            btn.textContent = String(item);
            btn.addEventListener('click', function () {
                estado[tablaId].pagina = item;
                render(tablaId);
            });
            nums.appendChild(btn);
        });
        botones.appendChild(nums);

        botones.appendChild(crearBotonTexto('Siguiente', e.pagina >= totalPaginas, function () {
            estado[tablaId].pagina++;
            render(tablaId);
        }));

        contenedor.appendChild(botones);
    }

    function iniciar(tablaId, opciones) {
        opciones = opciones || {};
        estado[tablaId] = { pageSize: opciones.pageSize || 8, pagina: 1 };
        render(tablaId);
    }

    function actualizar(tablaId, opciones) {
        opciones = opciones || {};
        if (!estado[tablaId]) {
            iniciar(tablaId, opciones);
            return;
        }
        if (opciones.resetPagina !== false) {
            estado[tablaId].pagina = 1;
        }
        render(tablaId);
    }

    return { iniciar: iniciar, actualizar: actualizar };
})();
