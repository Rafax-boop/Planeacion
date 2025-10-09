document.addEventListener('DOMContentLoaded', function () {
    const accionesList = document.getElementById('accionesList');
    const btnAgregar = document.getElementById('btnAgregarAccion');
    const btnEliminar = document.getElementById('btnEliminarAccion');
    let contadorAcciones = 3; // Ya empezamos con 3
    const MAX_ACCIONES = 6;
    const MIN_ACCIONES = 3;

    // Template para nueva acción
    function getAccionTemplate(numero) {
        return `
                    <div class="border rounded p-3 mb-3 accion-item" data-numero="${numero}">
                        <div class="row align-items-center mb-3">
                            <div class="col-md-12">
                                <h6 class="fw-semibold mb-0 titulo">${numero}. ACCIÓN</h6>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-12">
                                <label class="form-label fw-semibold small">DESCRIPCIÓN DE LA ACCIÓN</label>
                                <textarea class="form-control titulo input-arena" rows="3" placeholder="Describa la acción necesaria..."></textarea>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-4">
                                <label class="form-label fw-semibold small">FRECUENCIA DE REALIZACIÓN</label>
                                <select class="form-control titulo input-arena">
                                    <option value="">Seleccione la frecuencia</option>
                                    <option value="mensual">MENSUAL</option>
                                    <option value="anual">ANUAL</option>
                                    <option value="trimestral">TRIMESTRAL</option>
                                    <option value="semestral">SEMESTRAL</option>
                                    <option value="diaria">DIARIA</option>
                                    <option value="semanal">SEMANAL</option>
                                </select>
                            </div>
                            <div class="col-md-8">
                                <label class="form-label fw-semibold small">FECHA INICIO</label>
                                <input type="date" class="form-control titulo input-arena fecha-inicio">
                            </div>
                        </div>
                    </div>
                `;
    }

    // Actualizar números de acciones
    function actualizarNumeros() {
        document.querySelectorAll('.accion-item').forEach((accion, index) => {
            const numero = index + 1;
            accion.setAttribute('data-numero', numero);
            accion.querySelector('h6').textContent = `${numero}. ACCIÓN`;
        });
    }

    // Actualizar estado de botones
    function actualizarBotones() {
        btnAgregar.disabled = contadorAcciones >= MAX_ACCIONES;
        btnEliminar.disabled = contadorAcciones <= MIN_ACCIONES;
    }

    // Agregar acción
    btnAgregar.addEventListener('click', function () {
        if (contadorAcciones < MAX_ACCIONES) {
            contadorAcciones++;
            accionesList.insertAdjacentHTML('beforeend', getAccionTemplate(contadorAcciones));
            actualizarNumeros();
            actualizarBotones();
        }
    });

    // Eliminar acción
    btnEliminar.addEventListener('click', function () {
        if (contadorAcciones > MIN_ACCIONES) {
            document.querySelector('.accion-item:last-child').remove();
            contadorAcciones--;
            actualizarNumeros();
            actualizarBotones();
        }
    });

    // Inicializar botones
    actualizarBotones();
});