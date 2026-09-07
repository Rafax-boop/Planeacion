document.addEventListener('DOMContentLoaded', function () {
    const sidebar = document.querySelector('.sidebar');
    const toggle = document.getElementById('btnAccionesAdmin');
    const flyout = document.getElementById('submenuAccionesAdmin');
    if (!sidebar || !toggle || !flyout) return;

    function posicionarFlyout() {
        const rect = toggle.getBoundingClientRect();
        flyout.style.top = rect.top + 'px';
    }

    // Solo controla la visibilidad del panel (clase "open"); la clase
    // "active" la calcula el servidor según la ruta actual y no se toca aquí.
    function abrirFlyout() {
        posicionarFlyout();
        sidebar.classList.add('expanded-lock');
        toggle.classList.add('open');
        toggle.setAttribute('aria-expanded', 'true');
        flyout.classList.add('open');
    }

    function cerrarFlyout() {
        sidebar.classList.remove('expanded-lock');
        toggle.classList.remove('open');
        toggle.setAttribute('aria-expanded', 'false');
        flyout.classList.remove('open');
    }

    toggle.addEventListener('click', function (e) {
        e.stopPropagation();
        if (flyout.classList.contains('open')) {
            cerrarFlyout();
        } else {
            abrirFlyout();
        }
    });

    // Mientras el mouse esté sobre el submenú, el sidebar se mantiene
    // expandido (no depende de :hover sobre el <aside>, que se perdería
    // al mover el mouse hacia el panel flotante).
    flyout.addEventListener('mouseenter', function () {
        sidebar.classList.add('expanded-lock');
    });

    // Al elegir una opción, cerrar el submenú y colapsar el sidebar antes
    // de navegar a la página destino.
    flyout.querySelectorAll('.sidebar-flyout-item').forEach(function (item) {
        item.addEventListener('click', cerrarFlyout);
    });

    // Sin condicionar a que el flyout siga "open": si el clic fue fuera de
    // él y del botón, siempre se limpia (submenú Y bloqueo de expansión del
    // sidebar), aunque el submenú ya estuviera cerrado.
    document.addEventListener('click', function (e) {
        if (!flyout.contains(e.target) && !toggle.contains(e.target)) {
            cerrarFlyout();
        }
    });

    window.addEventListener('resize', function () {
        if (flyout.classList.contains('open')) posicionarFlyout();
    });
});
