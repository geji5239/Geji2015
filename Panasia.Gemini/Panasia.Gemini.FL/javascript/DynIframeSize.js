//自动调整iframe高度
function DynIframeSize(pTar) {
    //var pTar = null;
    //if (document.getElementById) {
    //    pTar = document.getElementById(iframe_id);
    //}
    //else {
    //    eval('pTar = ' + iframe_id + ';');
    //}
    alert(pTar);
    if (pTar && !window.opera) {
        //begin resizing iframe 
        pTar.style.display = "block"
        if (pTar.contentDocument && pTar.contentDocument.body.offsetHeight) {
            //ns6 syntax 
            pTar.height = pTar.contentDocument.body.offsetHeight + 10;
            //pTar.width = pTar.contentDocument.body.scrollWidth + 20;
        }
        else if (pTar.Document && pTar.Document.body.scrollHeight) {
            //ie5+ syntax 
            pTar.height = pTar.Document.body.scrollHeight;
            //pTar.width = pTar.Document.body.scrollWidth;
        }
    }
}