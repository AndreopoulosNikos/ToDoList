/**
 * Function to download the task table as an excel file
 * @param {any} fileName
 * @param {any} base64Data
 */
export function downloadFileFromBytes(fileName, base64Data) {
    const link = document.createElement('a');
    link.download = fileName;
    link.href = "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64," + base64Data;
    document.body.appendChild(link);
    link.click();
    link.remove();
}