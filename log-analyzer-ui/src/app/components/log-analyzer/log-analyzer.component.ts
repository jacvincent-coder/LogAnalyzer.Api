import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LogAnalyzerService } from '../../services/log-analyzer.service';
import { LogAnalysisResult } from '../../models/log-analysis-result';

@Component({
  selector: 'app-log-analyzer',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './log-analyzer.component.html',
  styleUrls: ['./log-analyzer.component.scss']
})
export class LogAnalyzerComponent {

  selectedFile: File | null = null;
  result: LogAnalysisResult | null = null;
  errorMessage: string | null = null;

  constructor(
    private logService: LogAnalyzerService,
    private cdr: ChangeDetectorRef
  ) {}

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files && input.files.length > 0 ? input.files[0] : null;
    console.log('[LogAnalyzer] File selected:', this.selectedFile?.name);
  }

  upload() {
    console.log('[LogAnalyzer] Upload clicked. Current file:', this.selectedFile);

    if (!this.selectedFile) {
      this.errorMessage = 'Please select a file.';
      this.result = null;
      this.cdr.detectChanges();   // ensure UI updates immediately
      return;
    }

    this.errorMessage = null;
    this.result = null;
    this.cdr.detectChanges();     // clear old result immediately

    this.logService.analyze(this.selectedFile).subscribe({
      next: (res) => {
        console.log('[LogAnalyzer] Response received:', res);
        this.result = res;
        this.errorMessage = null;

        // 🔑 Force Angular to update the view right now
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('[LogAnalyzer] Error from API:', err);
        this.errorMessage = 'Error uploading or analyzing file';
        this.result = null;
        this.cdr.detectChanges();
      }
    });
  }
}
