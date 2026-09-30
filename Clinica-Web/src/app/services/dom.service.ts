import { Injectable } from '@angular/core';
import $ from 'jquery';

declare var M: any;
@Injectable({
  providedIn: 'root'
})
export class DomService {
  vezMenu : boolean = true; 
  constructor(
  ) { }

  ShowSideBar() {    
    $("body").addClass("menu-open");
    $('.sidenav-overlay').attr('style', 'display: block; opacity: 1;');

    if(this.vezMenu)
    {
      $(".collapsible-body.active").parent().addClass("active").find(".collapsible-body").css("display", "block");
      this.vezMenu = false;
    }
  }

  RemoveMenu() {
    $("#menu-open").hide();
    $("#slide-out").addClass("validacion");
    $(".container-body").attr('style', 'left:0px; width: calc( 100% );');
  }

  HideSideBar(){
    $("body").removeClass('menu-open');
    $('.sidenav-overlay').removeAttr('style');
    $(".collapsible-body").css("display", "none");
    // $(".menu-li").removeClass("active");
  }

  ShowLoading(){
    $("#modal_loading_agro").show();
  }

  HideLoading()
  {
    $("#modal_loading_agro").hide();
  }

  ShowError(afterClose: () => void = () => {}) {
    var elem = document.getElementById('ptv-alert-error');    
    var opts = { 
      onCloseEnd: function(){     
       afterClose();
      }
    }
    var instance = M.Modal.init(elem, opts);
    instance.open();
  }


  
}
