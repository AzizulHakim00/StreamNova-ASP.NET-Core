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

    const headerSearch = document.querySelector('.header-search input');
    document.addEventListener('keydown', event => {
        if (event.key === '/' && headerSearch && !isTypingTarget(event.target)) {
            event.preventDefault();
            headerSearch.focus();
            headerSearch.select();
        }
    });

    setupWatchPage();
})();

function isTypingTarget(target) {
    return target instanceof HTMLInputElement
        || target instanceof HTMLTextAreaElement
        || target instanceof HTMLSelectElement
        || target?.isContentEditable;
}

function setupWatchPage() {
    const watchPage = document.querySelector('[data-watch-page]');
    if (!watchPage) return;

    const movieId = Number(watchPage.dataset.movieId || 0);
    const duration = Number(watchPage.dataset.duration || 1);
    const resumeAt = Math.max(0, Number(watchPage.dataset.resume || 0));
    const nextUrl = watchPage.dataset.nextUrl || '';
    const nextTitle = watchPage.dataset.nextTitle || '';
    const video = document.querySelector('#moviePlayer');
    const token = document.querySelector('#progressTokenForm input[name="__RequestVerificationToken"]')?.value || '';
    const speedSelect = document.querySelector('[data-playback-speed]');
    const pipButton = document.querySelector('[data-picture-in-picture]');
    const theaterButton = document.querySelector('[data-theater-mode]');
    const shareButton = document.querySelector('[data-share-watch]');
    const nextCard = document.querySelector('[data-next-title-card]');
    const nextCountdown = document.querySelector('[data-next-countdown]');
    const cancelNext = document.querySelector('[data-cancel-next]');
    const toast = document.querySelector('[data-watch-toast]');
    let lastSaved = 0;
    let nextTimer = null;
    let nextSeconds = 8;

    const showToast = message => {
        if (!toast) return;
        toast.textContent = message;
        toast.hidden = false;
        clearTimeout(showToast.timer);
        showToast.timer = setTimeout(() => { toast.hidden = true; }, 2400);
    };

    const saveGuestProgress = (progress, total) => {
        if (window.streamNovaWatch?.saveUrl) return;
        const state = JSON.parse(localStorage.getItem('streamnova-guest-progress') || '{}');
        state[movieId] = {
            progress: Math.floor(progress),
            duration: Math.floor(total || duration),
            updatedAt: Date.now()
        };
        localStorage.setItem('streamnova-guest-progress', JSON.stringify(state));
    };

    const getGuestResume = () => {
        if (window.streamNovaWatch?.saveUrl || resumeAt > 0) return resumeAt;
        const state = JSON.parse(localStorage.getItem('streamnova-guest-progress') || '{}');
        return Math.max(0, Number(state[movieId]?.progress || 0));
    };

    const saveProgress = async (progress, total) => {
        if (Math.abs(progress - lastSaved) < 10) return;
        lastSaved = progress;
        saveGuestProgress(progress, total);
        if (!window.streamNovaWatch?.saveUrl) return;

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

    const cancelAutoNext = () => {
        if (nextTimer) clearInterval(nextTimer);
        nextTimer = null;
        if (nextCard) nextCard.hidden = true;
        showToast('Auto-next cancelled');
    };

    const startAutoNext = () => {
        if (!nextUrl || !nextCard || nextTimer) return;
        nextSeconds = 8;
        nextCountdown.textContent = String(nextSeconds);
        nextCard.hidden = false;
        nextTimer = setInterval(() => {
            nextSeconds -= 1;
            nextCountdown.textContent = String(nextSeconds);
            if (nextSeconds <= 0) {
                clearInterval(nextTimer);
                window.location.assign(nextUrl);
            }
        }, 1000);
    };

    cancelNext?.addEventListener('click', cancelAutoNext);

    theaterButton?.addEventListener('click', () => {
        watchPage.classList.toggle('theater-mode');
        theaterButton.classList.toggle('active', watchPage.classList.contains('theater-mode'));
        showToast(watchPage.classList.contains('theater-mode') ? 'Theater mode on' : 'Theater mode off');
    });

    shareButton?.addEventListener('click', async () => {
        const shareData = { title: document.title, text: `Watch ${document.title.replace('Watching ', '')} on StreamNova`, url: window.location.href };
        try {
            if (navigator.share) {
                await navigator.share(shareData);
            } else {
                await navigator.clipboard.writeText(window.location.href);
                showToast('Watch link copied');
            }
        } catch {
            // The visitor can cancel the operating-system share sheet.
        }
    });

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

        speedSelect?.addEventListener('change', () => {
            video.playbackRate = Number(speedSelect.value || 1);
            showToast(`Playback speed ${speedSelect.value}×`);
        });

        pipButton?.addEventListener('click', async () => {
            try {
                if (document.pictureInPictureElement) {
                    await document.exitPictureInPicture();
                } else if (document.pictureInPictureEnabled) {
                    await video.requestPictureInPicture();
                } else {
                    showToast('Picture in picture is not supported');
                }
            } catch {
                showToast('Picture in picture is unavailable');
            }
        });

        video.addEventListener('loadedmetadata', () => {
            const storedResume = getGuestResume();
            if (storedResume > 0 && storedResume < video.duration - 10) {
                video.currentTime = storedResume;
                lastSaved = storedResume;
                showToast(`Resumed at ${formatClock(storedResume)}`);
            }
        });
        video.addEventListener('timeupdate', () => saveProgress(video.currentTime, video.duration));
        video.addEventListener('ended', () => {
            saveProgress(video.duration, video.duration);
            startAutoNext();
        });
        window.addEventListener('beforeunload', () => {
            saveProgress(video.currentTime, video.duration);
            hls?.destroy();
        });

        document.addEventListener('keydown', event => {
            if (isTypingTarget(event.target)) return;
            switch (event.key.toLowerCase()) {
                case ' ':
                case 'k':
                    event.preventDefault();
                    video.paused ? video.play() : video.pause();
                    break;
                case 'arrowleft':
                case 'j':
                    event.preventDefault();
                    video.currentTime = Math.max(0, video.currentTime - 10);
                    break;
                case 'arrowright':
                case 'l':
                    event.preventDefault();
                    video.currentTime = Math.min(video.duration || duration, video.currentTime + 10);
                    break;
                case 'm':
                    video.muted = !video.muted;
                    showToast(video.muted ? 'Muted' : 'Sound on');
                    break;
                case 'f':
                    if (document.fullscreenElement) document.exitFullscreen();
                    else video.requestFullscreen?.();
                    break;
                case 'n':
                    if (nextUrl) window.location.assign(nextUrl);
                    break;
            }
        });
        return;
    }

    document.addEventListener('keydown', event => {
        if (!isTypingTarget(event.target) && event.key.toLowerCase() === 'n' && nextUrl) {
            window.location.assign(nextUrl);
        }
    });
}

function formatClock(seconds) {
    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60).toString().padStart(2, '0');
    const secs = Math.floor(seconds % 60).toString().padStart(2, '0');
    return hours > 0 ? `${hours}:${minutes}:${secs}` : `${minutes}:${secs}`;
}
