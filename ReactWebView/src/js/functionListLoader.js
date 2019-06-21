const csv = require("csvtojson");
const csvFilePath = "../assets/CPUgenerator2_20190129_FunctionSummary.csv";

export async function readFunctionsList() {
    try {
        console.log("reading csv");
        const f = await csv().fromFile(csvFilePath);
        // alert(f);
        console.log("f ", f)
        return f;
    }
    catch(e) {
        return Error;
    }
}; 