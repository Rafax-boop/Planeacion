// Sub-menú flotante (flyout) de los ítems del sidebar que lo tengan.
// Usa position:fixed (ver sidebar.css) para escapar el overflow:hidden del
// <aside>, así que la posición real se calcula aquí según dónde esté el
// ítem en pantalla en el momento del hover.
document.addEventListener('DOMContentLoaded', function () {
    // Debe igualar la transición de "width" del .sidebar en sidebar.css:
    // el submenú solo aparece cuando el sidebar ya terminó de abrirse del
    // todo, no apenas empieza a expandirse.
    var RETRASO_APERTURA_SIDEBAR = 240;
    // Al salir de wrap o del flyout, el cierre se agenda con este retraso en
    // vez de ser inmediato: como el flyout es position:fixed y hay un hueco
    // (rect.right + 10px) entre el ítem y él, el mouse pasa por encima de
    // contenido que NO es descendiente de wrap al viajar hacia el flyout, lo
    // que dispara mouseleave antes de que el usuario llegue a hacer clic. El
    // retraso da tiempo a "aterrizar" en el flyout, que cancela el cierre.
    var RETRASO_CIERRE_FLYOUT = 300;

    function posicionarFlyout(wrap, flyout) {
        var rect = wrap.getBoundingClientRect();
        flyout.style.top = rect.top + 'px';
        flyout.style.left = (rect.right + 10) + 'px';
    }

    document.querySelectorAll('.sidebar-nav-item-wrap').forEach(function (wrap) {
        var flyout = wrap.querySelector('.sidebar-flyout');
        if (!flyout) return;

        var timerApertura = null;
        var timerCierre = null;

        function abrir() {
            clearTimeout(timerCierre);
            clearTimeout(timerApertura);
            timerApertura = setTimeout(function () {
                posicionarFlyout(wrap, flyout);
                flyout.classList.add('visible');
            }, RETRASO_APERTURA_SIDEBAR);
        }

        function programarCierre() {
            clearTimeout(timerApertura);
            clearTimeout(timerCierre);
            timerCierre = setTimeout(function () {
                flyout.classList.remove('visible');
            }, RETRASO_CIERRE_FLYOUT);
        }

        wrap.addEventListener('mouseenter', abrir);
        wrap.addEventListener('mouseleave', programarCierre);
        flyout.addEventListener('mouseenter', function () {
            clearTimeout(timerCierre);
        });
        flyout.addEventListener('mouseleave', programarCierre);
    });
});
