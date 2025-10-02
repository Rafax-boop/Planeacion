document.addEventListener('DOMContentLoaded', function () {
    const loginForm = document.getElementById('loginForm');
    const btnIngresar = document.getElementById('btnIngresar');

    loginForm.addEventListener('submit', function (e) {
        e.preventDefault();

        // Validar campos
        const usuario = document.getElementById('floatingUsuario').value;
        const password = document.getElementById('floatingPassword').value;

        if (!usuario || !password) {
            Swal.fire({
                icon: 'warning',
                title: 'Campos incompletos',
                text: 'Por favor ingrese usuario y contraseña',
                confirmButtonColor: '#741732'
            });
            return;
        }

        // Mostrar SweetAlert de carga
        showLoadingAlert();

        // Simular proceso de login (reemplazar con tu lógica real)
        simulateLogin(usuario, password);
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
            willOpen: () => {
                btnIngresar.disabled = true;
                btnIngresar.innerHTML = 'PROCESANDO...';
            }
        });
    }

    function simulateLogin(usuario, password) {
        // Simular llamada al servidor (reemplazar con tu API real)
        setTimeout(() => {
            Swal.close();

            // Aquí iría tu lógica real de autenticación
            // Por ahora simulamos éxito siempre
            Swal.fire({
                icon: 'success',
                title: '¡Bienvenido!',
                text: 'Ha iniciado sesión correctamente',
                confirmButtonColor: '#741732',
                willClose: () => {
                    // Redirigir al dashboard o página principal
                    window.location.href = '/Home/Index';
                }
            });

        }, 3000); // 3 segundos de simulación
    }

    // Restaurar botón si hay error
    function restoreButton() {
        btnIngresar.disabled = false;
        btnIngresar.innerHTML = 'INGRESAR';
    }
});