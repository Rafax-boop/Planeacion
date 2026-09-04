// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// ========================================
// TOAST DE ÉXITO (SweetAlert2, no bloqueante)
// ========================================
// Para avisos de "ya pasó" (guardado/editado/eliminado exitosamente). Los
// diálogos que piden una decisión (¿está seguro de eliminar/guardar?) siguen
// siendo Swal.fire modal normal, sin cambios.
const Toast = Swal.mixin({
    toast: true,
    position: 'top-end',
    timer: 2500,
    timerProgressBar: true,
    showConfirmButton: false,
    didOpen: (toast) => {
        toast.onmouseenter = Swal.stopTimer;
        toast.onmouseleave = Swal.resumeTimer;
    }
});

// ========================================
// CONFIRMACIÓN ANTES DE GUARDAR
// ========================================
function confirmarGuardado(mensaje) {
    return Swal.fire({
        title: '¿Confirmar guardado?',
        text: mensaje || 'Se guardarán los datos capturados.',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Sí, guardar',
        cancelButtonText: 'Cancelar',
        reverseButtons: true
    }).then((resultado) => resultado.isConfirmed);
}

// ========================================
// VALIDACIÓN DE CAMPOS REQUERIDOS EN MODALES
// ========================================
// campos: [{ label: 'Nombre', campoId: 'txtUsuario', valido: bool }]
// Muestra un SweetAlert con la lista de campos faltantes y, al aceptar,
// hace scroll + foco + resalta el primero. Devuelve true si no falta nada.
function validarCamposRequeridos(campos) {
    const faltantes = campos.filter(c => !c.valido);
    if (faltantes.length === 0) return true;

    Swal.fire({
        title: 'Campos obligatorios',
        html: 'Por favor completa los siguientes campos:<br><br>' +
              faltantes.map(c => '• ' + c.label).join('<br>'),
        icon: 'warning',
        confirmButtonText: 'Aceptar',
        allowOutsideClick: false
    }).then(() => enfocarCampo(faltantes[0].campoId));

    return false;
}

function enfocarCampo(campoId) {
    const campo = document.getElementById(campoId);
    if (!campo) return;

    const esSelect2 = campo.tagName === 'SELECT' && $(campo).hasClass('select2-hidden-accessible');

    // Select2 reduce el <select> original a una cajita de ~1x1px (oculta pero accesible);
    // scrollIntoView sobre ESE elemento no mueve nada. Hay que scrollear el widget visible
    // que Select2 genera justo al lado (".select2-container").
    const elementoVisible = esSelect2 ? $(campo).next('.select2-container')[0] : null;

    // 'smooth' se ve "trabado" al scrollear dentro de .app-content (no está acelerado
    // por el compositor como el scroll nativo de la ventana): en formularios largos la
    // animación tarda segundos en avanzar y la página parece congelada hasta el próximo
    // click, que fuerza el repintado. Salto instantáneo evita ese problema.
    (elementoVisible || campo).scrollIntoView({ behavior: 'auto', block: 'center' });

    setTimeout(() => {
        if (esSelect2) {
            // Los <select> con select2 quedan ocultos; se resalta el widget visible y se abre.
            const $widget = $(campo).next('.select2-container').find('.select2-selection');
            $widget.css({ 'border-color': '#dc3545', 'background-color': '#fff5f5' });
            $(campo).select2('open');
            setTimeout(() => $widget.css({ 'border-color': '', 'background-color': '' }), 3000);
        } else {
            campo.focus();
            campo.style.border = '2px solid #dc3545';
            campo.style.backgroundColor = '#fff5f5';
            setTimeout(() => {
                campo.style.border = '';
                campo.style.backgroundColor = '';
            }, 3000);
        }
    }, 100); // solo para dejar que el navegador repinte el salto instantáneo antes de resaltar
}

// Write your JavaScript code.
