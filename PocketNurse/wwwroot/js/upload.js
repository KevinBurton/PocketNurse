var Upload = function () {
    return {//these methods will be exposed through the Upload object
        Initialize: function () {
            var input = document.querySelector("input#uploadFile[type='file']");
            var preview = document.querySelector('.preview');

            input.addEventListener('change', updateImageDisplay);

            var fileTypes = [
                'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
            ];

            function updateImageDisplay() {
                while (preview.firstChild) {
                    preview.removeChild(preview.firstChild);
                }

                var curFiles = input.files;
                if (curFiles.length === 0) {
                    var para = document.createElement('p');
                    para.textContent = 'No files currently selected for upload';
                    preview.appendChild(para);
                } else {
                    var list = document.createElement('ol');
                    preview.appendChild(list);
                    for (var i = 0; i < curFiles.length; i++) {
                        var listItem = document.createElement('li');
                        if (validFileType(curFiles[i])) {
                            listItem.textContent = 'File name ' + curFiles[i].name + ', file size ' + returnFileSize(curFiles[i].size) + '.';
                        } else {
                            listItem.textContent = 'File name ' + curFiles[i].name + ': Not a valid file type. Update your selection.';
                        }

                        list.appendChild(listItem);
                    }
                }
            };
            function validFileType(file) {
                for (var i = 0; i < fileTypes.length; i++) {
                    if (file.type === fileTypes[i]) {
                        return true;
                    }
                }

                return false;
            };
            function returnFileSize(number) {
                if (number < 1024) {
                    return number + 'bytes';
                } else if (number >= 1024 && number < 1048576) {
                    return (number / 1024).toFixed(1) + 'KB';
                } else if (number >= 1048576) {
                    return (number / 1048576).toFixed(1) + 'MB';
                }
            };
        }
    }
};