import { Component, Inject, OnInit } from '@angular/core';
import { VotingPropertyService } from '../../services/voting-property.service';
import { Observable, Subject, from, firstValueFrom } from 'rxjs';
import { map, mergeMap, toArray } from 'rxjs/operators';
import { Router } from '@angular/router';
import { AppStateService } from '../../services/app-state.service';
import { FileInputListModel } from './file-input-list.model';
import { SecureDeleteService, SECURE_DELETE_SERVICE } from '../../services/secure-delete.service';
import { ElectronService } from '../../services/electron.service';
import { JSON_CONFIG_FILE, LOG_DIR, TEMP_DIR, VOTING_DATA_DIR } from '../../common/path.constants';
import { pathCombine } from '../../services/utils/path.utils';

@Component({
  selector: 'app-welcome-page',
  templateUrl: './welcome-page.component.html',
  styleUrls: ['./welcome-page.component.scss'],
})
export class WelcomePageComponent implements OnInit {
  private readonly fileList: FileInputListModel[] = [];

  public get enablePreview(): boolean {
    const isRunning = !!this.fileList.find(fileElement => fileElement.isRunning);
    const hasPostConfig = !!this.fileList.find(fileElement => fileElement.fileType === 'POST_CONFIG');
    const hasPostData = !!this.fileList.find(fileElement => fileElement.fileType === 'POST_CODE');
    const hasChvoteData = !!this.fileList.find(fileElement => fileElement.fileType === 'CHVOTE');
    const hasECH0045Data = !!this.fileList.find(fileElement => fileElement.fileType === 'ECH0045');
    const hasZip = !!this.fileList.find(fileElement => fileElement.fileType === 'ZIP');
    const hasCrypt = !!this.fileList.find(fileElement => fileElement.fileType === 'CRYPT');
    return !isRunning && !hasCrypt && hasZip && (hasChvoteData || (hasPostConfig && hasPostData) || hasECH0045Data);
  }

  private filePath = VOTING_DATA_DIR;

  public get fileGrouped(): any {
    const fileGroupedValue: any = [];
    const groups = new Set(this.fileList.map(item => item.fileType));
    groups.forEach(g =>
      fileGroupedValue.push({
        name: g,
        values: this.fileList.filter(i => i.fileType === g),
      }),
    );
    return fileGroupedValue.sort(function (a, b) {
      return a.name > b.name;
    });
  }

  constructor(
    private readonly votingPropertyService: VotingPropertyService,
    private readonly router: Router,
    private readonly appStateService: AppStateService,
    private readonly electronService: ElectronService,
    @Inject(SECURE_DELETE_SERVICE) private readonly secureDeleteService: SecureDeleteService,
  ) {}

  public async ngOnInit(): Promise<void> {
    // when a user lands on the welcome page it always resets the whole state and deletes all temp data.
    await this.sDelete();
    await this.electronService.createDirectory(LOG_DIR);
    await this.appStateService.init();
  }

  public async complete(): Promise<void> {
    await this.appStateService.update(s => (s.uploads = this.fileList.map(f => ({ fileType: f.fileType, filePath: f.filePath }))));
    this.router.navigate(['/prepare']);
  }

  public logger(value): void {
    console.log(value);
  }

  public addFiles(files: File[]): void {
    if (!files) {
      return;
    }

    from(files)
      .pipe(
        map((file: any) => {
          const tempValue: FileInputListModel = {
            filePath: pathCombine(this.filePath, file.name),
            file: file,
            fileType: 'OPEN',
            isRunning: true,
          };
          this.fileList.push(tempValue);
          return tempValue;
        }),
        mergeMap(fileObject => from(this.copyFile(fileObject.file)).pipe(map(() => fileObject))),
        toArray(),
      )
      .subscribe(value => {
        this.fileCheck(value);
      });
  }

  public async copyFile(file): Promise<void> {
    await this.electronService.createDirectory(this.filePath);
    await this.electronService.copyFile(file.path, pathCombine(this.filePath, file.name));
  }

  public async fileCheck(jobs?): Promise<void> {
    const fileJobs = jobs || this.fileList.filter(fileElement => fileElement.fileType === 'OPEN');
    if (fileJobs.length === 0) {
      return;
    }
    for (const jobItem of fileJobs) {
      jobItem.isRunning = true;
      let type: any = '0NONE';

      if (this.isZip(jobItem.file)) {
        this.removeZip();
        type = 'ZIP';
        jobItem.isRunning = true;
      }
      if (type === '0NONE' && this.isXml(jobItem.file)) {
        await this.readSomeLines(jobItem.file)
          .toPromise()
          .then(response => (type = this.checkXml(response)));
      }
      jobItem.fileType = type;
      jobItem.isRunning = false;
    }
    this.zipHandling();
  }

  private isZip(file: File): boolean {
    const validZipType = ['application/x-zip-compressed', 'application/zip'];
    return !!validZipType.find(zipType => zipType === file.type);
  }

  private removeZip(): void {
    const zipIndex = this.fileList.findIndex(item => item.fileType === 'ZIP');
    if (zipIndex !== -1) {
      this.fileList.splice(zipIndex, 1);
    }
  }

  private zipHandling(): void {
    const filesObject = this.fileList.find(item => item.fileType === 'ZIP');
    if (!filesObject) {
      return;
    }
    filesObject.isRunning = true;
    this.votingPropertyService.prepareProperty(filesObject.file).subscribe(() => {
      filesObject.isRunning = false;
      filesObject.filePath = JSON_CONFIG_FILE;
    });
  }

  private checkXml(xml): 'POST_CONFIG' | 'POST_CODE' | 'CHVOTE' | 'ECH0045' | '0NONE' {
    if (this.isPostConfig(xml)) {
      return 'POST_CONFIG';
    }
    if (this.isPostVoting(xml)) {
      return 'POST_CODE';
    }
    if (this.isChVote(xml)) {
      return 'CHVOTE';
    }
    if (this.iseCH0045(xml)) {
      return 'ECH0045';
    }
    return '0NONE';
  }

  private isXml(file: File): boolean {
    const validZipType = ['text/xml'];
    return !!validZipType.find(zipeType => zipeType === file.type);
  }

  private isPostConfig(xmlDom): boolean {
    const config = 'xmlns="http://www.evoting.ch/xmlns/config/';
    return xmlDom.includes(config);
  }

  private isPostVoting(xmlDom): boolean {
    const config = 'xmlns="http://www.evoting.ch/xmlns/print/';
    return xmlDom.includes(config);
  }

  private isChVote(xmlDom): boolean {
    const config = 'http://www.evote-ch.ch/common/';
    return xmlDom.includes(config);
  }

  private iseCH0045(xmlDom): boolean {
    const config = 'http://www.ech.ch/xmlns/eCH-0045';
    return xmlDom.includes(config);
  }

  public removeElement(fileName: string): void {
    const index = this.fileList.findIndex(element => element.file.name === fileName);
    if (index >= 0) {
      this.fileList.splice(index, 1);
    }
  }

  private readSomeLines(file): Observable<any> {
    const subject = new Subject();
    const decoder = new TextDecoder();
    let results: any = '';
    const fr = new FileReader();
    fr.onload = function (): void {
      results += decoder.decode(fr.result as ArrayBuffer, { stream: true });
      const lines = results.substring(0, 500);
      subject.next(lines);
      subject.complete();
    };
    fr.onerror = function (): void {
      subject.complete();
    };

    const slice = file.slice(0, 5000);
    fr.readAsArrayBuffer(slice);
    return subject;
  }

  private async sDelete(): Promise<void> {
    try {
      await firstValueFrom(this.secureDeleteService.deleteDirectory(TEMP_DIR, true));
    } catch (error) {
      console.error('error happened on sDelete', error);
    }
  }
}
