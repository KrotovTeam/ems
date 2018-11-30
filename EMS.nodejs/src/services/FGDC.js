const fs = require('fs');
const util = require('util');

function FGDCParse(data) {
    let startIndex = 0;
    let endIndex = data.length - 1;

    const START_SECTION_LABEL = 'GROUP = ';
    const END_SECTION_LABEL = 'END_GROUP = ';
    const LAST_SECTION_LABEL = 'END\n';


    let indexLevel = -1;
    let arrLinkLevel = [];
    const parserObj = {};
    let tempArr = '';
    let tempKey = '';


    function getTempLink() {
        let tempLink = parserObj;

        if (indexLevel === -1) { // Если еще нет секций в результате парсера
            return tempLink;
        }
        for (let i = 0; i <= indexLevel; i++) {
            tempLink = tempLink[arrLinkLevel[i]];
        }
        return tempLink;
    }

    let currentBeginLineIndex = startIndex;
    while (true) {
        // TODO Избавится от безконечного цикла!!!
        let currentLastLineIndex = data.indexOf('\n', currentBeginLineIndex);
        let lineFile = data.slice(currentBeginLineIndex, currentLastLineIndex);


        if (endIndex - currentLastLineIndex <= LAST_SECTION_LABEL.length) {
            break;
        }

        const indexStartSection = lineFile.indexOf(START_SECTION_LABEL);
        const indexEndSection = lineFile.indexOf(END_SECTION_LABEL);

        if (indexStartSection !== -1 && indexEndSection === -1) { // Если нашли начало секции
            let nameSection = lineFile.slice(indexStartSection + START_SECTION_LABEL.length, currentLastLineIndex);
            nameSection = normalizationKey(nameSection);
            let tempLink = getTempLink(); // Получаем ссылку на текущую секцию

            tempLink[nameSection] = {};
            arrLinkLevel.push(nameSection);
            indexLevel++;

        } else if (indexEndSection !== -1) { // Если конец секции
            indexLevel--;
            arrLinkLevel = arrLinkLevel.filter((el, i) => i <= indexLevel); // Возращаем список на секцию выше
        } else {// Если нашли внутренние(не делимые на секции) данные в секции
            let tempLink = getTempLink(); // Получаем ссылку на текущую секцию

            const splitArr = lineFile.split(' = ');
            const key = splitArr[0].replace(/\s+/g, ''); // Удаляем пробелы
            if (!splitArr[1]) { // если мы находимся внутри массива
                tempArr += key;
                if (key[key.length - 1] === ')') {
                    tempLink[normalizationKey(tempKey)] = stringToArray(tempArr);
                    tempArr = '';
                }
                currentBeginLineIndex = currentLastLineIndex + 1;
                continue;
            }

            const value = splitArr[1].replace(/"/g, ''); // Удаляем лишние ковычки
            if (value[0] === '(' && value[value.length - 1] !== ')') {// Если значение начало массива
                tempKey = key;
                tempArr += value;
                currentBeginLineIndex = currentLastLineIndex + 1;

                continue;
            }

            tempLink[normalizationKey(key)] = stringToArray(value);
        }
        currentBeginLineIndex = currentLastLineIndex + 1;
    }
    return parserObj;
}

function normalizationKey(key) {
    const arrKey = key.split('_');
    let res = '';

    for(let i =0;i<arrKey.length;i++){
        let a = arrKey[i].toLowerCase();
        let firstChar = arrKey[i][0].toUpperCase();

        res += firstChar + a.slice(1)
    }

    return res;
}


function stringToArray(str) {
    let returnStr = '';
    if (str[0] === '(' && str[str.length - 1] === ')') {
        returnStr = str.substr(1, str.length - 2); // Удаляем ковычки
        returnStr = returnStr.replace(/\s+/g, '');// Удаляем пробелы

        return returnStr.split(',');
    }
    return str;
}

async function parseWithJSON(path, nameFile){
    const readFile = util.promisify(fs.readFile);
    const appendFile = util.promisify(fs.appendFile);


    const data = await readFile(`${path}\\${nameFile}`, 'utf8');
    const parseData = FGDCParse(data, 'utf8');
    await appendFile(`${path}\\${nameFile.replace('.txt', '.json')}`, JSON.stringify(parseData));

}

module.exports = {
    FGDCParse,
    parseWithJSON
};

parseWithJSON('C:\\Users\\User\\Downloads\\Krim\\2016-06-22','LC08_L1TP_178029_20160622_20170323_01_T1_MTL.txt');
