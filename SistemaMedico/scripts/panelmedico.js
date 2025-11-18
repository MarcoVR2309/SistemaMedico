// Espera a que el DOM (la p�gina) est� completamente cargado
document.addEventListener("DOMContentLoaded", function () {

    // 1. Seleccionar todos los enlaces de navegaci�n que tienen 'data-target'
    const navLinks = document.querySelectorAll(".sidebar-nav .nav-item a[data-target], .sidebar-footer .nav-item a[data-target]");

    // 2. Seleccionar todos los paneles de contenido
    const contentPanels = document.querySelectorAll(".content-panel");

    // 3. Funci�n para manejar el clic en un enlace
    function showPanel(event) {
        // Prevenir el comportamiento por defecto del enlace (que recargar�a la p�gina)
        event.preventDefault();

        // Obtener el ID del panel objetivo (ej. "panel-dashboard")
        const targetId = this.getAttribute("data-target");

        // --- Actualizar Paneles de Contenido ---
        // Ocultar todos los paneles
        contentPanels.forEach(panel => {
            panel.classList.remove("active");
        });

        // Mostrar solo el panel objetivo
        const targetPanel = document.getElementById(targetId);
        if (targetPanel) {
            targetPanel.classList.add("active");
        }

        // --- Actualizar Estado Activo del Men� ---
        // Quitar la clase 'active' de todos los <li> padres de los enlaces
        navLinks.forEach(link => {
            link.parentElement.classList.remove("active");
        });

        // A�adir la clase 'active' solo al <li> padre del enlace clickeado
        this.parentElement.classList.add("active");
    }

    // 4. Asignar la funci�n 'showPanel' al evento 'click' de cada enlace
    navLinks.forEach(link => {
        link.addEventListener("click", showPanel);
    });

});
/* =========================================
 * L�GICA DEL MODAL (AGENDAR CITA)
 * ========================================= */

// Usamos el 'DOMContentLoaded' que ya tienes o agregamos uno nuevo
document.addEventListener("DOMContentLoaded", function () {

    // Seleccionamos los elementos del modal
    const modal = document.getElementById("modalAgendarCita");
    const openBtn = document.getElementById("btnAbrirModalCita");
    const closeBtn = document.getElementById("spanCerrarModal");
    const cancelBtn = document.getElementById("btnCancelarModal");

    // Funci�n para mostrar el modal
    function showModal(event) {
        event.preventDefault(); // Evita que el enlace <a> navegue
        if (modal) {
            modal.classList.add("show");
            // Limpia mensajes de error/�xito anteriores
            const msgLabel = modal.querySelector('.modal-mensaje');
            if (msgLabel) {
                msgLabel.classList.remove('show', 'success', 'error');
                msgLabel.innerText = '';
            }
        }
    }

    // Funci�n para ocultar el modal
    function hideModal() {
        if (modal) {
            modal.classList.remove("show");
        }
    }

    // Asignar eventos a los botones
    if (openBtn) {
        openBtn.addEventListener("click", showModal);
    }
    if (closeBtn) {
        closeBtn.addEventListener("click", hideModal);
    }
    if (cancelBtn) {
        cancelBtn.addEventListener("click", hideModal);
    }

    // Ocultar si se hace clic fuera del contenido
    if (modal) {
        modal.addEventListener("click", function (event) {
            // Si el clic fue en el fondo oscuro
            if (event.target === modal) {
                hideModal();
            }
        });
    }
});

// Funci�n global para que C# pueda llamarla
// (En caso de que el UpdatePanel no funcione bien)
function showModalFromCodeBehind(message, type) {
    const modal = document.getElementById("modalAgendarCita");
    if (modal) {
        modal.classList.add("show");
        const msgLabel = modal.querySelector('.modal-mensaje');
        if (msgLabel) {
            msgLabel.innerText = message;
            msgLabel.className = 'modal-mensaje show ' + type; // type es 'success' o 'error'
        }
    }
}
function hideModalFromCodeBehind() {
    const modal = document.getElementById("modalAgendarCita");
    if (modal) {
        modal.classList.remove("show");
    }
}

document.addEventListener("DOMContentLoaded", function () {

    // Seleccionamos los elementos del NUEVO modal
    const modal = document.getElementById("modalFinalizarConsulta");
    const closeBtn = document.getElementById("spanCerrarModalConsulta");
    const cancelBtn = document.getElementById("btnCancelarModalConsulta");

    // Funci�n para ocultar el modal
    function hideModal() {
        if (modal) {
            modal.classList.remove("show");
        }
    }

    // Asignar eventos
    if (closeBtn) {
        closeBtn.addEventListener("click", hideModal);
    }
    if (cancelBtn) {
        cancelBtn.addEventListener("click", hideModal);
    }
    if (modal) {
        modal.addEventListener("click", function (event) {
            if (event.target === modal) {
                hideModal();
            }
        });
    }
});

// Funci�n global para que C# pueda LLAMARLA
function showFinalizarModal() {
    const modal = document.getElementById("modalFinalizarConsulta");
    if (modal) {
        modal.classList.add("show");
        // Limpia mensajes de error/�xito anteriores
        const msgLabel = modal.querySelector('.modal-mensaje');
        if (msgLabel) {
            msgLabel.classList.remove('show', 'success', 'error');
            msgLabel.innerText = '';
        }
    }
}

// Funci�n global para que C# pueda LLAMARLA
function hideFinalizarModal() {
    const modal = document.getElementById("modalFinalizarConsulta");
    if (modal) {
        modal.classList.remove("show");
    }
}