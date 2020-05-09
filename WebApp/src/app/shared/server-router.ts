/** @format */

import { Router } from '@angular/router';

export function changeServer(router: Router, server: string) {
  const urlsplit = router.url.substr(1).split('/');
  urlsplit[0] = server.toLowerCase();
  router.navigateByUrl(urlsplit.join('/'));
}
