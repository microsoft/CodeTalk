const WebSocket = require('ws')

const wss = new WebSocket.Server({ port: 9001 })
const react = new WebSocket.Server({ port: 9003 })

let memoryHandler = undefined;

react.on('connection', _memoryHandler => {
  console.log("memory Connected."); 
  memoryHandler = _memoryHandler;
});

wss.on('connection', ws => {
  ws.on('message', message => {
    // console.log(message);
    if(memoryHandler){
      memoryHandler.send(message);
    }
  })
  ws.send('ho!')
})

const wssC = new WebSocket.Server({ port: 9005 });
const cpu = new WebSocket.Server({ port: 9007 });

let cpuHandler = undefined;

cpu.on('connection', _cpuHandler => {
  console.log("CPU Connected."); 
  cpuHandler = _cpuHandler;
});

wssC.on('connection', ws => {
  ws.on('message', message => {
    // console.log(message);
    if(cpuHandler){
      cpuHandler.send(message);
    }
  })
  ws.send('ho!')
})

const wssF = new WebSocket.Server({ port: 6000 });
const func = new WebSocket.Server({ port: 6002 });

let funcHandler = undefined;

func.on('connection', _funcHandler => {
  console.log("Function Connected."); 
  funcHandler = _funcHandler;
});

wssF.on('connection', ws => {
  console.log("function view connected");
  ws.on('message', message => {
    console.log(message);
    if(funcHandler){
      funcHandler.send(message);
    }
  })
  ws.send('ho!')
})