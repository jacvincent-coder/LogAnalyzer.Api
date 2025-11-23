import { Component, ChangeDetectorRef, ViewChild, ElementRef } from '@angular/core';
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

  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  selectedFile: File | null = null;
  result: LogAnalysisResult | null = null;
  errorMessage: string | null = null;
  loading = false;
  showError = false;

  constructor(
    private logService: LogAnalyzerService,
    private cdr: ChangeDetectorRef
  ) {}

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files && input.files.length > 0 ? input.files[0] : null;

    console.log('[LogAnalyzer] File selected:', this.selectedFile?.name);
  }

  private showErrorBanner(msg: string) {
    this.errorMessage = msg;
    this.showError = true;

    setTimeout(() => {
      this.showError = false;
      this.cdr.detectChanges();
    }, 4000);
  }

  reset() {
    console.log('[LogAnalyzer] Reset clicked');

    this.selectedFile = null;
    this.result = null;
    this.errorMessage = null;
    this.loading = false;
    this.showError = false;

    if (this.fileInput) {
      this.fileInput.nativeElement.value = '';
    }

    // Ensure UI sync
    this.cdr.detectChanges();
  }

  downloadResult(result: LogAnalysisResult) {
  const fileName = 'log-analysis-result.json';

  const blob = new Blob([JSON.stringify(result, null, 2)], {
    type: 'application/json'
  });

  const url = window.URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = fileName;

  document.body.appendChild(a);
  a.click();
  document.body.removeChild(a);

  window.URL.revokeObjectURL(url);
}


  upload() {
    console.log('[LogAnalyzer] Upload clicked. Current file:', this.selectedFile);

    if (!this.selectedFile) {
      this.showErrorBanner('Please select a file.');
      this.result = null;
      this.cdr.detectChanges();
      return;
    }

    this.loading = true;
    this.errorMessage = null;
    this.result = null;
    this.cdr.detectChanges();

    this.logService.analyze(this.selectedFile).subscribe({
      next: (res) => {
        console.log('[LogAnalyzer] Response received:', res);
        this.result = res;
        this.loading = false;
        this.errorMessage = null;

        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('[LogAnalyzer] Error from API:', err);
        this.showErrorBanner('Error uploading or analyzing file');
        this.result = null;
        this.loading = false;

        this.cdr.detectChanges();
      }
    });
  }
}
