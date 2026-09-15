// ========================================
// HELPERS PARA FETCH: manejo de sesión expirada y respuestas no JSON
// ========================================

class SesionExpiradaError extends Error {
    constructor() {
        super('La sesión ha expirado');
        this.name = 'SesionExpiradaError';
    }
}

function verificarRespuesta(response) {
    if (response.redirected && response.url.includes('/Acceso/Login')) {
        throw new SesionExpiradaError();
    }

    if (!response.ok) {
        throw new Error(`Error HTTP: ${response.status}`);
    }

    const contentType = response.headers.get('content-type') || '';
    if (!contentType.includes('application/json')) {
        throw new Error('Respuesta inesperada del servidor');
    }
}

function obtenerURLReLogin() {
    const returnUrl = encodeURIComponent(window.location.pathname + window.location.search);
    return `/Acceso/Login?ReturnUrl=${returnUrl}`;
}

function manejarSesionExpirada() {
    Swal.fire({
        title: 'Sesión expirada',
        html: `
            <div style="text-align: left;">
                <p>Tu sesión ha expirado por inactividad.</p>
                <p>Por favor, inicia sesión nuevamente para continuar.</p>
            </div>
        `,
        icon: 'warning',
        confirmButtonText: 'Ir a iniciar sesión',
        confirmButtonColor: '#3085d6',
        allowOutsideClick: false,
        allowEscapeKey: false
    }).then(() => {
        window.location.href = obtenerURLReLogin();
    });
}

function redirigirLogin() {
    window.location.href = obtenerURLReLogin();
}