const catalog = [
  {id:'bbb',title:'Big Buck Bunny',year:2008,genre:'Animation',duration:'10 min',kind:'full',type:'video',url:'https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4',source:'https://studio.blender.org/projects/big-buck-bunny/',license:'Creative Commons · Blender Foundation',description:'A gentle giant of a rabbit turns the tables on three mischievous forest bullies.',a:'#7c2d12',b:'#f59e0b'},
  {id:'elephants',title:'Elephants Dream',year:2006,genre:'Animation',duration:'11 min',kind:'full',type:'video',url:'https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4',source:'https://studio.blender.org/projects/elephants-dream/',license:'Creative Commons · Blender Foundation',description:'Two characters explore a strange machine-filled world in the first Blender open movie.',a:'#172554',b:'#6366f1'},
  {id:'sintel',title:'Sintel',year:2010,genre:'Fantasy',duration:'15 min',kind:'full',type:'video',url:'https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/Sintel.mp4',source:'https://studio.blender.org/projects/sintel/',license:'Creative Commons · Blender Foundation',description:'A determined young woman crosses a dangerous fantasy world while searching for a dragon.',a:'#3f1d38',b:'#e11d48'},
  {id:'tears',title:'Tears of Steel',year:2012,genre:'Sci-Fi',duration:'12 min',kind:'full',type:'video',url:'https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/TearsOfSteel.mp4',source:'https://mango.blender.org/',license:'Creative Commons Attribution 3.0',description:'Scientists and warriors reunite in Amsterdam to prevent a destructive robot conflict.',a:'#0c4a6e',b:'#22d3ee'},
  {id:'spring',title:'Spring',year:2019,genre:'Fantasy',duration:'8 min',kind:'full',type:'video',url:'https://commons.wikimedia.org/wiki/Special:Redirect/file/Spring_-_Blender_Open_Movie.webm',source:'https://studio.blender.org/films/spring/',license:'Creative Commons Attribution 4.0',description:'A shepherd and her dog confront ancient spirits to restore the cycle of the seasons.',a:'#14532d',b:'#eab308'},
  {id:'sprite',title:'Sprite Fright',year:2021,genre:'Animation',duration:'11 min',kind:'full',type:'video',url:'https://commons.wikimedia.org/wiki/Special:Redirect/file/Sprite_Fright_-_Open_Movie_by_Blender_Studio.webm',source:'https://studio.blender.org/films/sprite-fright/',license:'Creative Commons Attribution 4.0',description:'Teenagers discover that a peaceful woodland community is far more dangerous than it appears.',a:'#052e16',b:'#84cc16'},
  {id:'cosmos',title:'Cosmos Laundromat',year:2015,genre:'Fantasy',duration:'12 min',kind:'full',type:'video',url:'https://commons.wikimedia.org/wiki/Special:Redirect/file/Cosmos_Laundromat_-_First_Cycle_-_Official_Blender_Foundation_release.webm',source:'https://studio.blender.org/projects/cosmos-laundromat/',license:'Creative Commons Attribution 3.0',description:'A sheep meets a mysterious salesman who offers him a life-changing journey.',a:'#2e1065',b:'#ec4899'},
  {id:'coffee',title:'Coffee Run',year:2020,genre:'Comedy',duration:'3 min',kind:'full',type:'video',url:'https://commons.wikimedia.org/wiki/Special:Redirect/file/Coffee_Run_-_Blender_Open_Movie-full_movie.webm',source:'https://studio.blender.org/films/coffee-run/',license:'Creative Commons Attribution 4.0',description:'A woman races through memories of a relationship while trying to reach her morning coffee.',a:'#451a03',b:'#f59e0b'},
  {id:'charge',title:'Charge',year:2022,genre:'Sci-Fi',duration:'5 min',kind:'full',type:'video',url:'https://commons.wikimedia.org/wiki/Special:Redirect/file/Charge_-_Blender_Open_Movie-full_movie.webm',source:'https://studio.blender.org/films/charge/',license:'Creative Commons Attribution 4.0',description:'An old robot faces one final confrontation over a precious source of energy.',a:'#083344',b:'#22d3ee'},
  {id:'wing',title:'WING IT!',year:2023,genre:'Comedy',duration:'4 min',kind:'full',type:'video',url:'https://commons.wikimedia.org/wiki/Special:Redirect/file/WING_IT%21_-_Blender_Open_Movie-full_movie.webm',source:'https://studio.blender.org/films/wing-it/',license:'Creative Commons Attribution 4.0',description:'An inexperienced engineer and an ambitious pilot improvise through a chaotic flight test.',a:'#1e3a8a',b:'#facc15'},
  {id:'interstellar',title:'Interstellar',year:2014,genre:'Sci-Fi',duration:'Official trailer',kind:'trailer',type:'youtube',youtube:'zSWdZVtXT7E',source:'https://www.paramountpictures.com/movies/interstellar',license:'Official promotional trailer',description:'Explorers travel through a wormhole in space in an attempt to ensure humanity’s survival.',a:'#0f172a',b:'#64748b'},
  {id:'blade',title:'Blade Runner 2049',year:2017,genre:'Sci-Fi',duration:'Official trailer',kind:'trailer',type:'youtube',youtube:'gCcx85zbxz4',source:'https://www.warnerbros.com/movies/blade-runner-2049',license:'Official promotional trailer',description:'A young officer uncovers a long-buried secret that leads him to a former blade runner.',a:'#431407',b:'#fb923c'},
  {id:'shutter',title:'Shutter Island',year:2010,genre:'Thriller',duration:'Official trailer',kind:'trailer',type:'youtube',youtube:'v8yrZSkKxTA',source:'https://www.paramountpictures.com/movies/shutter-island',license:'Official promotional trailer',description:'A marshal investigates a disappearance from an isolated hospital and questions everything.',a:'#172554',b:'#0ea5e9'},
  {id:'fastx',title:'Fast X',year:2023,genre:'Action',duration:'Official trailer',kind:'trailer',type:'youtube',youtube:'SAhlmquynBY',source:'https://www.fastxmovie.com/',license:'Official promotional trailer',description:'A street-racing family faces a powerful enemy connected to its past.',a:'#450a0a',b:'#ef4444'},
  {id:'barbie',title:'Barbie',year:2023,genre:'Comedy',duration:'Official trailer',kind:'trailer',type:'youtube',youtube:'pBk4NYhWNMM',source:'https://www.barbie-themovie.com/',license:'Official promotional trailer',description:'A journey into the real world challenges a perfect life and reveals the power of choice.',a:'#831843',b:'#f472b6'},
  {id:'spiderman',title:'Spider-Man',year:2002,genre:'Action',duration:'Official trailer',kind:'trailer',type:'youtube',youtube:'t06RUxPbp_c',source:'https://www.sonypictures.com/movies/spiderman',license:'Official promotional trailer',description:'A young hero learns that great ability comes with responsibility.',a:'#172554',b:'#dc2626'}
];

const grid = document.querySelector('#catalogGrid');
const searchInput = document.querySelector('#searchInput');
const filters = document.querySelector('#filters');
const count = document.querySelector('#resultCount');
const empty = document.querySelector('#emptyState');
const dialog = document.querySelector('#playerDialog');
const mount = document.querySelector('#playerMount');
const favoritesButton = document.querySelector('#favoritesButton');
const favoriteCount = document.querySelector('#favoriteCount');
let activeFilter = 'all';
let favoritesOnly = false;
let deferredInstall;
const favorites = new Set(JSON.parse(localStorage.getItem('streamnova-favorites') || '[]'));

function matches(item) {
  const query = searchInput.value.trim().toLowerCase();
  const textMatch = !query || `${item.title} ${item.genre} ${item.year} ${item.description}`.toLowerCase().includes(query);
  const filterMatch = activeFilter === 'all' || item.kind === activeFilter || item.genre.toLowerCase().replace(/[^a-z]/g,'') === activeFilter;
  return textMatch && filterMatch && (!favoritesOnly || favorites.has(item.id));
}

function render() {
  const items = catalog.filter(matches);
  grid.innerHTML = items.map(item => `
    <article class="card" style="--a:${item.a};--b:${item.b}">
      <div class="art"><span class="kind ${item.kind}">${item.kind === 'full' ? 'Full movie' : 'Trailer'}</span><strong class="art-title">${item.title}</strong></div>
      <button class="play-card" type="button" data-play="${item.id}" aria-label="Play ${item.title}"><span>▶</span></button>
      <button class="favorite ${favorites.has(item.id) ? 'saved' : ''}" type="button" data-favorite="${item.id}" aria-label="Save ${item.title}">${favorites.has(item.id) ? '♥' : '♡'}</button>
      <div class="card-body"><h3>${item.title}</h3><div class="card-meta"><span>${item.year} · ${item.genre}</span><span>${item.duration}</span></div><p>${item.description}</p></div>
    </article>`).join('');
  count.textContent = `${items.length} title${items.length === 1 ? '' : 's'}`;
  empty.hidden = items.length !== 0;
  favoriteCount.textContent = favorites.size;
}

function openPlayer(item) {
  mount.replaceChildren();
  const media = item.type === 'youtube' ? document.createElement('iframe') : document.createElement('video');
  if (item.type === 'youtube') {
    media.src = `https://www.youtube-nocookie.com/embed/${item.youtube}?rel=0&autoplay=1&playsinline=1`;
    media.allow = 'autoplay; encrypted-media; picture-in-picture; fullscreen';
    media.allowFullscreen = true;
    media.title = `${item.title} official trailer`;
  } else {
    media.src = item.url;
    media.controls = true;
    media.autoplay = true;
    media.playsInline = true;
    media.preload = 'metadata';
  }
  mount.append(media);
  document.querySelector('#playerBadge').textContent = item.kind === 'full' ? 'Full open movie' : 'Official trailer';
  document.querySelector('#playerTitle').textContent = item.title;
  document.querySelector('#playerDescription').textContent = item.description;
  document.querySelector('#playerMeta').textContent = `${item.year} · ${item.genre} · ${item.license}`;
  const source = document.querySelector('#playerSource');
  source.href = item.source;
  if (typeof dialog.showModal === 'function') dialog.showModal(); else dialog.setAttribute('open','');
}

function closePlayer() {
  mount.replaceChildren();
  if (dialog.open && typeof dialog.close === 'function') dialog.close(); else dialog.removeAttribute('open');
}

filters.addEventListener('click', event => {
  const button = event.target.closest('[data-filter]');
  if (!button) return;
  activeFilter = button.dataset.filter;
  favoritesOnly = false;
  filters.querySelectorAll('button').forEach(item => item.classList.toggle('active', item === button));
  render();
});
searchInput.addEventListener('input', render);
grid.addEventListener('click', event => {
  const favoriteButton = event.target.closest('[data-favorite]');
  if (favoriteButton) {
    const id = favoriteButton.dataset.favorite;
    favorites.has(id) ? favorites.delete(id) : favorites.add(id);
    localStorage.setItem('streamnova-favorites', JSON.stringify([...favorites]));
    render();
    return;
  }
  const playButton = event.target.closest('[data-play]');
  if (playButton) openPlayer(catalog.find(item => item.id === playButton.dataset.play));
});
favoritesButton.addEventListener('click', () => { favoritesOnly = !favoritesOnly; favoritesButton.classList.toggle('active', favoritesOnly); render(); document.querySelector('#catalog').scrollIntoView(); });
document.querySelector('#closePlayer').addEventListener('click', closePlayer);
dialog.addEventListener('click', event => { if (event.target === dialog) closePlayer(); });
document.addEventListener('keydown', event => { if (event.key === 'Escape') closePlayer(); });
document.querySelector('#featuredPlay').addEventListener('click', () => openPlayer(catalog[0]));
document.querySelector('#surpriseButton').addEventListener('click', () => openPlayer(catalog[Math.floor(Math.random() * catalog.length)]));

window.addEventListener('beforeinstallprompt', event => { event.preventDefault(); deferredInstall = event; document.querySelector('#installButton').hidden = false; });
document.querySelector('#installButton').addEventListener('click', async () => { if (!deferredInstall) return; deferredInstall.prompt(); await deferredInstall.userChoice; deferredInstall = null; document.querySelector('#installButton').hidden = true; });
window.addEventListener('offline', () => { const banner = document.createElement('div'); banner.className = 'offline-banner'; banner.textContent = 'You are offline — the catalog shell is still available.'; document.body.append(banner); });
window.addEventListener('online', () => document.querySelector('.offline-banner')?.remove());
if ('serviceWorker' in navigator) window.addEventListener('load', () => navigator.serviceWorker.register('./sw.js'));
render();
