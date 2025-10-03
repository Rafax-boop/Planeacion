document.addEventListener('DOMContentLoaded', function () {
    const loginForm = document.getElementById('loginForm');
    const btnIngresar = document.getElementById('btnIngresar');
    const usuarioInput = document.getElementById('floatingUsuario');
    const passwordInput = document.getElementById('floatingPassword');

    // Verificar si hay mensaje de error del servidor
    if (window.mensajeError && window.mensajeError !== '') {
        Swal.fire({
            icon: 'error',
            title: 'Error de autenticación',
            text: window.mensajeError,
            confirmButtonColor: '#741732'
        });
    }

    loginForm.addEventListener('submit', function (e) {
        // Validar campos antes de enviar
        const usuario = usuarioInput.value.trim();
        const password = passwordInput.value.trim();

        if (!usuario || !password) {
            e.preventDefault(); // Solo prevenir si hay error
            Swal.fire({
                icon: 'warning',
                title: 'Campos incompletos',
                text: 'Por favor ingrese usuario y contraseña',
                confirmButtonColor: '#741732'
            });
            return;
        }

        // Si todo está bien, mostrar loading y permitir el envío
        showLoadingAlert();        
    });

    function showLoadingAlert() {
        Swal.fire({
            title: 'Verificando credenciales',
            html: `
                <div class="my-4">
                    <img src="/img/familias-dif.png" alt="Cargando" 
                         class="loading-pulse" 
                         style="height: 80px; width: auto;">
                </div>
                <p class="text-muted">Por favor espere...</p>
            `,
            background: 'rgba(255, 255, 255, 0.85)',
            backdrop: 'rgba(192, 152, 102, 0.1)',
            showConfirmButton: false,
            allowOutsideClick: false,
            customClass: {
                popup: 'glass-alert'
            },
            didOpen: () => {
                btnIngresar.disabled = true;
                btnIngresar.innerHTML = 'PROCESANDO...';
            }
        });
    }
});