/**
 * Celestryll — include.js
 * Injects shared header.html and footer.html into every page.
 * Requires a web server (e.g. `npx serve .` or GitHub Pages) — fetch() does
 * not work from the file:// protocol.
 */
(function () {
  'use strict';

  async function loadPartial(placeholderId, file) {
    const el = document.getElementById(placeholderId);
    if (!el) return;
    try {
      const res = await fetch(file);
      if (!res.ok) throw new Error('HTTP ' + res.status);
      el.outerHTML = await res.text();
      if (placeholderId === 'header-placeholder') afterHeaderLoad();
      if (placeholderId === 'footer-placeholder')  afterFooterLoad();
    } catch (err) {
      console.warn('[Celestryll] Could not load partial:', file, '—', err.message);
    }
  }

  function afterHeaderLoad() {
    // Mobile nav toggle
    const toggle = document.getElementById('nav-toggle');
    const menu   = document.getElementById('nav-menu');
    if (toggle && menu) {
      toggle.addEventListener('click', function () {
        const open = menu.classList.toggle('open');
        toggle.classList.toggle('open', open);
        toggle.setAttribute('aria-expanded', String(open));
      });
      menu.querySelectorAll('a').forEach(function (a) {
        a.addEventListener('click', function () {
          menu.classList.remove('open');
          toggle.classList.remove('open');
          toggle.setAttribute('aria-expanded', 'false');
        });
      });
    }

    // Highlight the active nav link
    const current = window.location.pathname.split('/').pop() || 'index.html';
    document.querySelectorAll('.nav-link').forEach(function (link) {
      const href = (link.getAttribute('href') || '').split('/').pop();
      if (href === current || (current === '' && href === 'index.html')) {
        link.classList.add('active');
      }
    });
  }

  function afterFooterLoad() {
    // Set the current year in the footer copyright
    const yr = document.getElementById('footer-year');
    if (yr) yr.textContent = new Date().getFullYear();
  }

  document.addEventListener('DOMContentLoaded', function () {
    loadPartial('header-placeholder', 'header.html');
    loadPartial('footer-placeholder', 'footer.html');
  });
})();
