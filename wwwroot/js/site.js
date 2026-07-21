(() => {
    const header = document.querySelector('#siteHeader');
    if (header) {
        const updateHeader = () => header.classList.toggle('scrolled', window.scrollY > 30);
        updateHeader();
        window.addEventListener('scroll', updateHeader, { passive: true });
    }

    const profileToggle = document.querySelector('[data-profile-toggle]');
    const profileMenu = document.querySelector('[data-profile-menu]');
    if (profileToggle && profileMenu) {
        profileToggle.addEventListener('click', () => profileMenu.classList.toggle('open'));
        document.addEventListener('click', event => {
            if (!profileToggle.contains(event.target) && !profileMenu.contains(event.target)) {
                profileMenu.classList.remove('open');
            }
        });
    }

    setupWatchPage();
})();

function setupWatchPage() {
    const watchPage = document.querySelector('[data-watch-page]');
    if (!watchPage) return;

    const movieId = Number(watchPage.dataset.movieId || 0);
    const duration = Number(watchPage.dataset.duration || 1);
    const resumeAt = Math.max(0, Number(watchPage.dataset.resume || 0));
    const video = document.querySelector('#moviePlayer');
    const token = document.querySelector('#progressTokenForm input[name="__RequestVerificationToken"]')?.value || '';
    let lastSaved = 0;

    const saveProgress = async (progress, total) => {
        if (!window.streamNovaWatch?.saveUrl || Math.abs(progress - lastSaved) < 10) return;
        lastSaved = progress;
        const body = new URLSearchParams({
            id: String(movieId),
            progressSeconds: String(Math.floor(progress)),
            durationSeconds: String(Math.floor(total || duration)),
            __RequestVerificationToken: token
        });
        try {
            await fetch(window.streamNovaWatch.saveUrl, {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body
            });
        } catch {
            // Playback should continue even when progress cannot be saved.
        }
    };

    if (video) {
        const hlsSource = video.dataset.hlsSource;
        let hls = null;

        if (hlsSource) {
            if (video.canPlayType('application/vnd.apple.mpegurl')) {
                video.src = hlsSource;
            } else if (window.Hls?.isSupported()) {
                hls = new window.Hls();
                hls.loadSource(hlsSource);
                hls.attachMedia(video);
            } else {
                video.insertAdjacentHTML('afterend', '<p class="player-error">This browser cannot play this HLS stream.</p>');
            }
        }

        video.addEventListener('loadedmetadata', () => {
            if (resumeAt > 0 && resumeAt < video.duration - 10) {
                video.currentTime = resumeAt;
                lastSaved = resumeAt;
            }
        });
        video.addEventListener('timeupdate', () => saveProgress(video.currentTime, video.duration));
        window.addEventListener('beforeunload', () => {
            saveProgress(video.currentTime, video.duration);
            hls?.destroy();
        });
        return;
    }

    const player = document.querySelector('[data-demo-player]');
    const fill = document.querySelector('[data-progress-fill]');
    const label = document.querySelector('[data-time-label]');
    const toggles = document.querySelectorAll('[data-play-toggle]');
    if (!player || !fill || !label || toggles.length === 0) return;

    let playing = false;
    let progress = Math.min(duration, resumeAt);
    lastSaved = progress;
    let timer = null;

    const formatTime = seconds => {
        const minutes = Math.floor(seconds / 60).toString().padStart(2, '0');
        const secs = Math.floor(seconds % 60).toString().padStart(2, '0');
        return `${minutes}:${secs}`;
    };

    const render = () => {
        fill.style.width = `${Math.min(100, (progress / duration) * 100)}%`;
        label.textContent = `${formatTime(progress)} / ${formatTime(duration)}`;
        toggles.forEach(button => button.textContent = playing ? '❚❚' : '▶');
    };

    const toggle = () => {
        playing = !playing;
        if (playing) {
            timer = window.setInterval(() => {
                progress = Math.min(duration, progress + 1);
                render();
                saveProgress(progress, duration);
                if (progress >= duration) toggle();
            }, 1000);
        } else if (timer) {
            clearInterval(timer);
            timer = null;
            saveProgress(progress, duration);
        }
        render();
    };

    toggles.forEach(button => button.addEventListener('click', toggle));
    render();
}