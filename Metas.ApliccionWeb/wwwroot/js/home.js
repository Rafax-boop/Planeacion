(function () {
    var DIAS = ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'];
    var MESES = ['enero', 'febrero', 'marzo', 'abril', 'mayo', 'junio', 'julio', 'agosto', 'septiembre', 'octubre', 'noviembre', 'diciembre'];

    function saludoPorHora(hora) {
        if (hora < 12) return 'Buenos días';
        if (hora < 19) return 'Buenas tardes';
        return 'Buenas noches';
    }

    function pad(numero) {
        return String(numero).padStart(2, '0');
    }

    function actualizarReloj() {
        var ahora = new Date();

        var hh = document.getElementById('home-clock-hh');
        var mm = document.getElementById('home-clock-mm');
        var ss = document.getElementById('home-clock-ss');
        var fecha = document.getElementById('home-date-text');
        var saludo = document.getElementById('home-greeting-text');

        if (hh) hh.textContent = pad(ahora.getHours());
        if (mm) mm.textContent = pad(ahora.getMinutes());
        if (ss) ss.textContent = pad(ahora.getSeconds());
        if (fecha) fecha.textContent = DIAS[ahora.getDay()] + ', ' + ahora.getDate() + ' de ' + MESES[ahora.getMonth()] + ' de ' + ahora.getFullYear();
        if (saludo) saludo.textContent = saludoPorHora(ahora.getHours());
    }

    actualizarReloj();
    setInterval(actualizarReloj, 1000);
})();
