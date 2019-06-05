import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';

import { GlobalService } from '../shared/services/global.service';
import { FullNodeApiService } from '../shared/services/fullnode.api.service';
import { ModalService } from '../shared/services/modal.service';

import { WalletLoad } from '../shared/models/wallet-load';
import { ThemeService } from '../shared/services/theme.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {
  constructor(private globalService: GlobalService, private FullNodeApiService: FullNodeApiService, private genericModalService: ModalService, private router: Router, private fb: FormBuilder, private themeService: ThemeService) {
    this.buildDecryptForm();
    this.isDarkTheme = themeService.getCurrentTheme().themeType == 'dark';
  }

  public hasWallet: boolean = false;
  public isDecrypting = false;
  public isDarkTheme = false;
  private openWalletForm: FormGroup;
  private wallets: [string];

  ngOnInit() {
    this.getWalletFiles();
    this.getCurrentNetwork();
  }

  private buildDecryptForm(): void {
    this.openWalletForm = this.fb.group({
      "selectWallet": [{ value: "", disabled: this.isDecrypting }, Validators.required],
      "password": [{ value: "", disabled: this.isDecrypting }, Validators.required]
    });

    this.openWalletForm.valueChanges
      .subscribe(data => this.onValueChanged(data));

    this.onValueChanged();
  }

  onValueChanged(data?: any) {
    if (!this.openWalletForm) { return; }
    const form = this.openWalletForm;
    for (const field in this.formErrors) {
      this.formErrors[field] = '';
      const control = form.get(field);
      if (control && control.dirty && !control.valid) {
        const messages = this.validationMessages[field];
        for (const key in control.errors) {
          this.formErrors[field] += messages[key] + ' ';
        }
      }
    }
  }

  formErrors = {
    'password': ''
  };

  validationMessages = {
    'password': {
      'required': 'Please enter your password.'
    }
  };

  private getWalletFiles() {
    this.FullNodeApiService.getWalletFiles()
      .subscribe(
        response => {
          this.wallets = response.walletsFiles;
          this.globalService.setWalletPath(response.walletsPath);
          if (this.wallets.length > 0) {
            this.hasWallet = true;
            for (let wallet in this.wallets) {
              this.wallets[wallet] = this.wallets[wallet].slice(0, -12);
            }
          } else {
            this.hasWallet = false;
          }
        }
      );
  }

  public onCreateClicked() {
    this.router.navigate(['setup']);
  }

  public onEnter() {
    if (this.openWalletForm.valid) {
      this.onDecryptClicked();
    }
  }

  public onDecryptClicked() {
    this.isDecrypting = true;
    this.globalService.setWalletName("x42ServerMain");
    let walletLoad = new WalletLoad(
      "x42ServerMain",
      this.openWalletForm.get("password").value
    );
    this.loadWallet(walletLoad);
  }

  private loadWallet(walletLoad: WalletLoad) {
    this.FullNodeApiService.loadX42Wallet(walletLoad)
      .subscribe(
        response => {
          this.router.navigate(['wallet/dashboard']);
        },
        error => {
          this.isDecrypting = false;
        }
      );
  }

  private getCurrentNetwork() {
    this.FullNodeApiService.getNodeStatus()
      .subscribe(
        response => {
          let responseMessage = response;
          this.globalService.setCoinUnit(responseMessage.coinTicker);
          this.globalService.setNetwork(responseMessage.network);
        }
      );
  }
}