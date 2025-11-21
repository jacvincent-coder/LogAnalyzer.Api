import { TestBed } from '@angular/core/testing';

import { LogAnalyzerService } from './log-analyzer.service';

describe('LogAnalyzer', () => {
  let service: LogAnalyzerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LogAnalyzerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
