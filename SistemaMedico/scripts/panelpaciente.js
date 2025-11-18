// ===========================================================================
// SCRIPTS PARA PANEL PACIENTE - VERSIÓN FINAL
// Compatible con la estructura de PanelMedico
// ===========================================================================

console.log('🚀 Cargando PanelPaciente.js...');

// Esperar a que el DOM esté completamente listo
document.addEventListener("DOMContentLoaded", function () {
    console.log('✅ DOM Cargado - Inicializando...');

    inicializarNavegacion();
    inicializarModal();
});

// ===========================================================================
// NAVEGACIÓN ENTRE PANELES
// ===========================================================================

function inicializarNavegacion() {
    console.log('📱 Inicializando navegación...');

    // Seleccionar TODOS los enlaces con data-target
    const navLinks = document.querySelectorAll(".sidebar-nav .nav-item a[data-target], .sidebar-footer .nav-item a[data-target]");
    const contentPanels = document.querySelectorAll(".content-panel");

    console.log('   - Enlaces encontrados:', navLinks.length);
    console.log('   - Paneles encontrados:', contentPanels.length);

    navLinks.forEach(function (link) {
        link.addEventListener("click", function (event) {
            event.preventDefault();

            const targetId = this.getAttribute("data-target");
            console.log('🎯 Cambiando a panel:', targetId);

            // Ocultar todos los paneles
            contentPanels.forEach(function (panel) {
                panel.classList.remove("active");
            });

            // Mostrar el panel objetivo
            const targetPanel = document.getElementById(targetId);
            if (targetPanel) {
                targetPanel.classList.add("active");
                console.log('   ✅ Panel mostrado:', targetId);
            } else {
                console.error('   ❌ Panel no encontrado:', targetId);
            }

            // Actualizar estado activo del menú
            navLinks.forEach(function (l) {
                l.parentElement.classList.remove("active");
            });
            this.parentElement.classList.add("active");
        });
    });
}

// ===========================================================================
// MODAL DE AGENDAR CITA
// ===========================================================================

function inicializarModal() {
    console.log('📋 Inicializando modal...');

    const modal = document.getElementById("modalAgendarCita");
    const openBtn = document.getElementById("btnAbrirModalCita");
    const closeBtn = document.getElementById("spanCerrarModal");
    const cancelBtn = document.getElementById("btnCancelarModal");

    console.log('   - Modal:', modal ? '✅' : '❌');
    console.log('   - Botón Abrir:', openBtn ? '✅' : '❌');
    console.log('   - Botón Cerrar:', closeBtn ? '✅' : '❌');
    console.log('   - Botón Cancelar:', cancelBtn ? '✅' : '❌');

    // Abrir modal
    if (openBtn) {
        openBtn.addEventListener("click", function (event) {
            event.preventDefault();
            console.log('🔓 Abriendo modal...');
            showModal();
        });
    }

    // Cerrar modal con la X
    if (closeBtn) {
        closeBtn.addEventListener("click", function () {
            console.log('❌ Cerrando modal (X)...');
            hideModal();
        });
    }

    // Cerrar modal con botón Cancelar
    if (cancelBtn) {
        cancelBtn.addEventListener("click", function () {
            console.log('❌ Cerrando modal (Cancelar)...');
            hideModal();
        });
    }

    // Cerrar modal al hacer clic fuera
    if (modal) {
        modal.addEventListener("click", function (event) {
            if (event.target === modal) {
                console.log('❌ Cerrando modal (click fuera)...');
                hideModal();
            }
        });
    }
}

// Función para mostrar el modal
function showModal() {
    const modal = document.getElementById("modalAgendarCita");
    if (modal) {
        modal.classList.add("show");

        // Limpiar mensajes de error/éxito anteriores
        const msgLabel = modal.querySelector('.modal-mensaje');
        if (msgLabel) {
            msgLabel.classList.remove('show', 'success', 'error');
            msgLabel.innerText = '';
        }

        console.log('   ✅ Modal visible');
    }
}

// Función para ocultar el modal
function hideModal() {
    const modal = document.getElementById("modalAgendarCita");
    if (modal) {
        modal.classList.remove("show");
        console.log('   ✅ Modal oculto');
    }
}

// ===========================================================================
// FUNCIONES GLOBALES (Llamadas desde C#)
// ===========================================================================

// Mostrar modal desde CodeBehind con mensaje
function showModalFromCodeBehind(message, type) {
    console.log('🔔 Llamada desde C#:', message, type);
    const modal = document.getElementById("modalAgendarCita");
    if (modal) {
        modal.classList.add("show");
        const msgLabel = modal.querySelector('.modal-mensaje');
        if (msgLabel && message && type) {
            msgLabel.innerText = message;
            msgLabel.className = 'modal-mensaje show ' + type;
        }
    }
}

// Ocultar modal desde CodeBehind
function hideModalFromCodeBehind() {
    console.log('🔔 Ocultando modal desde C#');
    hideModal();
}

// ===========================================================================
// FUNCIONES AUXILIARES
// ===========================================================================

function verDetalleCita(idCita) {
    console.log('📄 Ver detalle de cita:', idCita);
    alert('Función de detalle de cita en desarrollo.\nID: ' + idCita);
}

function handleGuardarCita(button) {
    if (button.disabled) {
        return false;
    }
    button.disabled = true;
    button.textContent = 'Guardando...';
    setTimeout(function () {
        button.disabled = false;
        button.textContent = 'Reservar Cita';
    }, 3000);
    return true;
}

document.addEventListener("DOMContentLoaded", function () {
    const txtFecha = document.querySelector('input[id*="txtFechaCita"]');

    if (txtFecha) {
        txtFecha.addEventListener('input', function (e) {
            let valor = e.target.value.replace(/\D/g, ''); // Solo números

            if (valor.length >= 2) {
                valor = valor.substring(0, 2) + '/' + valor.substring(2);
            }
            if (valor.length >= 5) {
                valor = valor.substring(0, 5) + '/' + valor.substring(5, 9);
            }

            e.target.value = valor;
        });
    }
});

console.log('PanelPaciente.js cargado completamente');