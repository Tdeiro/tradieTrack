import { http, HttpResponse } from 'msw';

export const handlers = [
  http.get('/api/users', () => {
    return HttpResponse.json([
      { id: 'u1', organizationId: '11111111-1111-1111-1111-111111111111', email: 'anna@example.com', fullName: 'Anna', createdAt: new Date().toISOString() },
      { id: 'u2', organizationId: '11111111-1111-1111-1111-111111111111', email: 'ben@example.com',  fullName: 'Ben',  createdAt: new Date().toISOString() }
    ]);
  }),
];
