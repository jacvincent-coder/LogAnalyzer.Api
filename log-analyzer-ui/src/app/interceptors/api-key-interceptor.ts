import { HttpInterceptorFn } from '@angular/common/http';

export const ApiKeyInterceptor: HttpInterceptorFn = (req, next) => {
  const cloned = req.clone({
    setHeaders: {
      'x-api-key': '9b955bef42a8ecd1c3530863ad0a40922edc2afe0c444215b6968f29af13da5d'
    }
  });

  return next(cloned);
};
