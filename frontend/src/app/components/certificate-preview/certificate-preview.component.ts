import { Component, Inject, Input, OnInit } from '@angular/core';
import { E_VOTING_CONFIG_DIR } from '../../common/path.constants';
import { Certificate } from '../../models/certificate.model';
import { CRYPTIC_SERVICE, CrypticService } from '../../services/cryptic.service';
import { pathCombine } from '../../services/utils/path.utils';

@Component({
  selector: 'app-certificate-preview',
  templateUrl: './certificate-preview.component.html',
  styleUrls: ['./certificate-preview.component.scss'],
})
export class CertificatePreviewComponent implements OnInit {
  @Input()
  public certificatePaths: string[] = [];

  public certificates: Certificate[] = [];

  constructor(@Inject(CRYPTIC_SERVICE) private readonly crypticService: CrypticService) {}

  public ngOnInit(): void {
    this.crypticService.getReceiverCertificates(this.certificatePaths.map(r => pathCombine(E_VOTING_CONFIG_DIR, r))).subscribe(list => {
      if (!list) {
        return;
      }

      this.certificates = list;
    });
  }
}
