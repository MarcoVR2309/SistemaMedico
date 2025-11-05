// Esperamos a que TODO (HTML, CSS, imágenes) esté cargado
window.addEventListener("load", () => {

    // 1. Apuntamos a la lista del carrusel (el nuevo .hero-lista)
    const lista = document.querySelector(".hero-lista");

    // Verificamos que exista antes de ejecutar
    if (lista) {
        
        // --- Configuración Inicial ---
        let index = 1; // Empezamos en el 1er slide REAL
        const autoPlayTime = 5000; // 2 segundos
        const transitionSpeed = 'transform 0.8s ease-in-out';
        const noTransition = 'transform 0s';
        
        // Variable para guardar nuestro 'setInterval'
        let autoPlayInterval = null;

        // --- Lógica de Clones ---
        let totalSlidesOriginales = lista.children.length;
        let cloneFirst = lista.children[0].cloneNode(true);
        lista.appendChild(cloneFirst);
        let cloneLast = lista.children[totalSlidesOriginales - 1].cloneNode(true);
        lista.insertBefore(cloneLast, lista.children[0]);
        const totalSlides = lista.children.length;

        // --- Posición Inicial ---
        lista.style.transition = noTransition;
        lista.style.transform = `translateX(${-index * 100}%)`;
        lista.offsetWidth; // Forzamos al navegador a aplicar el cambio
        lista.style.transition = transitionSpeed;


        // --- Lógica de Movimiento ---
        function slideRight() {
            index++;
            lista.style.transition = transitionSpeed;
            lista.style.transform = `translateX(${-index * 100}%)`;
        }

        // --- Lógica del Bucle Infinito (Reinicia al final de la transición) ---
        lista.addEventListener('transitionend', () => {
            // Si llegamos al clon del final (que es copia del slide 1)
            if (index === totalSlides - 1) { 
                lista.style.transition = noTransition;
                index = 1; // Saltamos al slide 1 real
                lista.style.transform = `translateX(${-index * 100}%)`;
            }

            // Si llegamos al clon del inicio (que es copia del último slide)
            if (index === 0) {
                lista.style.transition = noTransition;
                index = totalSlides - 2; // Saltamos al último slide real
                lista.style.transform = `translateX(${-index * 100}%)`;
            }
        });

        // --- Funciones para Iniciar y Pausar ---
        function startAutoPlay() {
            // Si ya hay un intervalo, lo limpiamos por si acaso
            clearInterval(autoPlayInterval);
            // Creamos el intervalo y lo guardamos en la variable
            autoPlayInterval = setInterval(slideRight, autoPlayTime);
        }

        function pauseAutoPlay() {
            // Limpiamos el intervalo
            clearInterval(autoPlayInterval);
        }

        // --- ¡LA SOLUCIÓN! (Page Visibility API) ---
        // Escuchamos el evento 'visibilitychange'
        document.addEventListener("visibilitychange", () => {
            if (document.hidden) {
                // Si la página se oculta (cambias de pestaña)
                pauseAutoPlay();
            } else {
                // Si la página se vuelve visible (regresas)
                startAutoPlay();
            }
        });

        // --- Iniciar el Carrusel por primera vez ---
        startAutoPlay();
    }
});