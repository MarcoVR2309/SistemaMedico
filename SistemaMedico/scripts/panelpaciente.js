// JavaScript
function changeTab(tabId) {
    var tabButtons = document.querySelectorAll('.tab-button');
    var contentSections = document.querySelectorAll('.content-section');

    tabButtons.forEach(btn => btn.classList.remove('active'));
    contentSections.forEach(section => section.classList.remove('active'));

    var btnTab = document.getElementById('btn_' + tabId);
    var contentTab = document.getElementById(tabId);

    if (btnTab) btnTab.classList.add('active');
    if (contentTab) contentTab.classList.add('active');

    var hdnField = document.getElementById('hdnActiveTab');
    if (hdnField) {
        hdnField.value = tabId;
    }
}