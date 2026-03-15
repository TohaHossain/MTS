import { datadogRum } from '@datadog/browser-rum';
import { reactPlugin } from '@datadog/browser-rum-react';
                    
datadogRum.init({
    applicationId: 'abfd62d8-d48c-4592-9135-8486d38a98fd',
    clientToken: 'pub95389e76020d32ceeee9a705dcf35d82',
    site: 'datadoghq.com',
    service: 'tradingsim-frontend',
    env: 'local',
    version: '0.1.0',
    sessionSampleRate: 100,
    sessionReplaySampleRate: 100,
    trackResources: true,
    trackUserInteractions: true,
    trackLongTasks: true,
    plugins: [reactPlugin({ router: false })],
});