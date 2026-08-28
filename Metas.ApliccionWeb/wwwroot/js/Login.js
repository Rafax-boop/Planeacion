document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.toggle-password').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var input = document.getElementById(btn.getAttribute('data-target'));
            if (!input) return;

            var showing = input.type === 'password';
            input.type = showing ? 'text' : 'password';

            var icon = btn.querySelector('i');
            icon.classList.toggle('fa-eye');
            icon.classList.toggle('fa-eye-slash');

            btn.setAttribute('aria-label', showing ? 'Ocultar contraseña' : 'Mostrar contraseña');
        });
    });
});
