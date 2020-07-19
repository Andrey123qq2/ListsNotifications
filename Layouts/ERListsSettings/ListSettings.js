function checkBoxSingleMailHandler() {
    var checkBox = this.event.srcElement;

    var textBoxName = checkBox.getAttribute("name").replace("checkBoxField", "subject");

    var textBox = document.getElementsByName(textBoxName)[0];

    if (checkBox.checked) {
        //textBox.disabled = false;
        textBox.readOnly = false;
        textBox.style.borderColor = '#ababab';
    } else {
        //textBox.disabled = true;
        textBox.readOnly = true;
        textBox.style.borderColor = '#e1e1e1'
        textBox.value = "";
        textBox.setAttribute("value", "");
    }
    //textBox.disabled = (checkBox.checked) ? false : true;
}