// Espera a que el DOM (la página) esté completamente cargado
document.addEventListener("DOMContentLoaded", function () {

    // 1. Seleccionar todos los enlaces de navegación que tienen 'data-target'
    const navLinks = document.querySelectorAll(".sidebar-nav .nav-item a[data-target], .sidebar-footer .nav-item a[data-target]");

    // 2. Seleccionar todos los paneles de contenido
    const contentPanels = document.querySelectorAll(".content-panel");

    // 3. Función para manejar el clic en un enlace
    function showPanel(event) {
        // Prevenir el comportamiento por defecto del enlace (que recargaría la página)
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

        // --- Actualizar Estado Activo del Menú ---
        // Quitar la clase 'active' de todos los <li> padres de los enlaces
        navLinks.forEach(link => {
            link.parentElement.classList.remove("active");
        });

        // Añadir la clase 'active' solo al <li> padre del enlace clickeado
        this.parentElement.classList.add("active");
    }

    // 4. Asignar la función 'showPanel' al evento 'click' de cada enlace
    navLinks.forEach(link => {
        link.addEventListener("click", showPanel);
    });

});
/* =========================================
 * LÓGICA DEL MODAL (AGENDAR CITA)
 * ========================================= */

// Usamos el 'DOMContentLoaded' que ya tienes o agregamos uno nuevo
document.addEventListener("DOMContentLoaded", function () {

    // Seleccionamos los elementos del modal
    const modal = document.getElementById("modalAgendarCita");
    const openBtn = document.getElementById("btnAbrirModalCita");
    const closeBtn = document.getElementById("spanCerrarModal");
    const cancelBtn = document.getElementById("btnCancelarModal");

    // Función para mostrar el modal
    function showModal(event) {
        event.preventDefault(); // Evita que el enlace <a> navegue
        if (modal) {
            modal.classList.add("show");
            // Limpia mensajes de error/éxito anteriores
            const msgLabel = modal.querySelector('.modal-mensaje');
            if (msgLabel) {
                msgLabel.classList.remove('show', 'success', 'error');
                msgLabel.innerText = '';
            }
        }
    }

    // Función para ocultar el modal
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

// Función global para que C# pueda llamarla
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

    // Función para ocultar el modal
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

// Función global para que C# pueda LLAMARLA
function showFinalizarModal() {
    const modal = document.getElementById("modalFinalizarConsulta");
    if (modal) {
        modal.classList.add("show");
        // Limpia mensajes de error/éxito anteriores
        const msgLabel = modal.querySelector('.modal-mensaje');
        if (msgLabel) {
            msgLabel.classList.remove('show', 'success', 'error');
            msgLabel.innerText = '';
        }
    }
}

// Función global para que C# pueda LLAMARLA
function hideFinalizarModal() {
    const modal = document.getElementById("modalFinalizarConsulta");
    if (modal) {
        modal.classList.remove("show");
    }
}

/* =========================================
 * LÓGICA DEL MODAL (VER FICHA)
 * ========================================= */
document.addEventListener("DOMContentLoaded", function () {
    const modal = document.getElementById("modalVerFicha");
    const closeBtn = document.getElementById("spanCerrarModalFicha");
    const closeBtnBottom = document.getElementById("btnCerrarFichaInferior");

    function hideModal() {
        if (modal) modal.classList.remove("show");
    }

    if (closeBtn) closeBtn.addEventListener("click", hideModal);
    if (closeBtnBottom) closeBtnBottom.addEventListener("click", hideModal);

    if (modal) {
        modal.addEventListener("click", function (event) {
            if (event.target === modal) hideModal();
        });
    }
});

// Función global para C#
function showFichaModal() {
    const modal = document.getElementById("modalVerFicha");
    if (modal) {
        modal.classList.add("show");
    }
}
// =========================================
// FUNCIÓN PARA DESCARGAR PDF
// =========================================
function descargarFichaPDF() {
    // 1. Seleccionamos el elemento que queremos imprimir
    const elemento = document.getElementById('areaImprimibleFicha');

    // 2. Obtenemos el nombre del paciente para el nombre del archivo
    // (Buscamos el span/label donde pusiste el nombre)
    const nombrePaciente = document.getElementById('lblFichaPaciente') ?
        document.getElementById('lblFichaPaciente').innerText : 'Paciente';

    // 3. Configuraciones del PDF
    const opciones = {
        margin: 0.5, // Margen en pulgadas
        filename: `Ficha_Medica_${nombrePaciente}.pdf`, // Nombre del archivo
        image: { type: 'jpeg', quality: 0.98 },
        html2canvas: { scale: 2, useCORS: true }, // scale 2 mejora la calidad
        jsPDF: { unit: 'in', format: 'a4', orientation: 'portrait' }
    };

    // 4. Generar y Descargar
    // (El botón cambiará de texto para avisar que está procesando)
    const btnPDF = document.querySelector('.btn-service-outline.btn-teal');
    const textoOriginal = btnPDF.innerHTML;
    btnPDF.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Generando...';

    html2pdf().set(opciones).from(elemento).save().then(function () {
        // Restaurar botón cuando termine
        btnPDF.innerHTML = textoOriginal;
    });
}