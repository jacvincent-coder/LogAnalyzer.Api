import { TestBed } from '@angular/core/testing';
import { LogAnalyzerComponent } from './log-analyzer.component';

describe('LogAnalyzerComponent', () => {

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LogAnalyzerComponent]
    }).compileComponents();
  });

  it('should create', () => {
    const fixture = TestBed.createComponent(LogAnalyzerComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });

});
