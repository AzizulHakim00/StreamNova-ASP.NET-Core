(() => {
    const randomButton = document.querySelector('[data-random-play]');
    if (randomButton) {
        randomButton.addEventListener('click', () => {
            const links = [...document.querySelectorAll('[data-catalog-movie][data-playable="true"] .movie-card-actions a:first-child')]
                .filter(link => link instanceof HTMLAnchorElement && link.href);
            if (links.length === 0) return;
            const selected = links[Math.floor(Math.random() * links.length)];
            selected.classList.add('catalog-random-pick');
            randomButton.textContent = `▶ ${selected.closest('.movie-card')?.querySelector('h3')?.textContent || 'Playing'}`;
            window.setTimeout(() => window.location.assign(selected.href), 500);
        });
    }

    const rails = document.querySelectorAll('.discovery-rail');
    rails.forEach(rail => {
        rail.addEventListener('wheel', event => {
            if (Math.abs(event.deltaY) <= Math.abs(event.deltaX)) return;
            event.preventDefault();
            rail.scrollLeft += event.deltaY;
        }, { passive: false });
    });
})();
