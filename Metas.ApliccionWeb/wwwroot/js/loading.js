(function () {
    var PASOS = [
        { en: 150, hasta: 25 },
        { en: 600, hasta: 55 },
        { en: 1600, hasta: 78 }
    ];
    var TIEMPO_MAXIMO_OVERLAY = 6000;

    var barra = document.getElementById('progress-bar-top');
    var overlay = document.getElementById('loading-overlay');
    var timersBarra = [];
    var timerOverlay = null;
    var timerCompletar = null;

    function limpiarTimersBarra() {
        timersBarra.forEach(function (id) { clearTimeout(id); });
        timersBarra = [];
        clearTimeout(timerCompletar);
    }

    function iniciarBarra() {
        if (!barra) return;
        limpiarTimersBarra();
        barra.style.width = '0%';
        void barra.offsetWidth; // fuerza reflow para reiniciar la transición
        barra.classList.add('active');

        PASOS.forEach(function (paso) {
            var id = setTimeout(function () {
                barra.style.width = paso.hasta + '%';
            }, paso.en);
            timersBarra.push(id);
        });
    }

    // Sin navegación pjax, la barra se limpia sola al descargarse la página;
    // esta función queda disponible para quien quiera "completarla" a mano.
    function completarBarra() {
        if (!barra) return;
        limpiarTimersBarra();
        barra.style.width = '100%';
        timerCompletar = setTimeout(function () {
            barra.classList.remove('active');
            timerCompletar = setTimeout(function () {
                barra.style.width = '0%';
            }, 200);
        }, 150);
    }

    function ocultarBarra() {
        if (!barra) return;
        limpiarTimersBarra();
        barra.classList.remove('active');
        barra.style.width = '0%';
    }

    function mostrarOverlay() {
        if (!overlay) return;
        overlay.classList.add('active');
        clearTimeout(timerOverlay);
        timerOverlay = setTimeout(function () {
            overlay.classList.remove('active');
        }, TIEMPO_MAXIMO_OVERLAY);
    }

    function ocultarOverlay() {
        if (!overlay) return;
        clearTimeout(timerOverlay);
        overlay.classList.remove('active');
    }

    // API pública para quien quiera disparar el efecto a mano (ej. una
    // petición AJAX puntual con $.get/$.post).
    window.AppLoading = {
        iniciarBarra: iniciarBarra,
        completarBarra: completarBarra,
        ocultarBarra: ocultarBarra,
        mostrarOverlay: mostrarOverlay,
        ocultarOverlay: ocultarOverlay
    };

    // Links con data-loading-overlay (ej. generar PDF/Excel en pestaña nueva)
    // no descargan la página actual, así que se manejan aquí directamente.
    document.addEventListener('click', function (e) {
        if (e.defaultPrevented || e.button !== 0 || e.metaKey || e.ctrlKey || e.shiftKey || e.altKey) return;

        var link = e.target.closest('a');
        if (link && link.hasAttribute('data-loading-overlay')) {
            mostrarOverlay();
        }
    });

    // Cualquier navegación normal (link o formulario) muestra la barra;
    // se limpia sola al descargarse la página.
    document.addEventListener('click', function (e) {
        if (e.defaultPrevented || e.button !== 0 || e.metaKey || e.ctrlKey || e.shiftKey || e.altKey) return;

        var link = e.target.closest('a');
        if (!link || !link.href) return;
        if (link.target && link.target !== '_self') return;
        if (link.hasAttribute('download')) return;
        var href = link.getAttribute('href') || '';
        if (href === '' || href.charAt(0) === '#' || href.indexOf('mailto:') === 0 || href.indexOf('tel:') === 0 || href.indexOf('javascript:') === 0) return;
        if (link.origin !== window.location.origin) return;

        iniciarBarra();
    });

    document.addEventListener('submit', function (e) {
        var form = e.target;
        if (!(form instanceof HTMLFormElement)) return;
        if (form.target && form.target !== '_self') return;

        iniciarBarra();

        var submitter = e.submitter;
        if (form.hasAttribute('data-loading-overlay') || (submitter && submitter.hasAttribute('data-loading-overlay'))) {
            mostrarOverlay();
        }
    });

    // Si el usuario vuelve con "atrás" (bfcache) hay que resetear todo, si no se queda pegado.
    window.addEventListener('pageshow', function () {
        ocultarBarra();
        ocultarOverlay();
    });
})();
