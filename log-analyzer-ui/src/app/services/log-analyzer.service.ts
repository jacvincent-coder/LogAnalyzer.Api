import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LogAnalysisResult } from '../models/log-analysis-result';

@Injectable({ providedIn: 'root' })
export class LogAnalyzerService {
  private baseUrl = 'https://localhost:7219/api/Log';

  constructor(private http: HttpClient) {}

analyze(file: File) {
  const formData = new FormData();
  formData.append('logFile', file);

  return this.http.post<LogAnalysisResult>(`${this.baseUrl}/analyze`, formData);
}

}
