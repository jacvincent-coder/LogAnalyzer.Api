import { Component } from '@angular/core';
import { LogAnalyzerComponent } from './components/log-analyzer/log-analyzer.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [LogAnalyzerComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {}
