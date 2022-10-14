
# Xiudong-Playwright

基于C#与Playwright模拟浏览器抢秀动门票


## 环境

.NET 6 

## Features

✅模拟抢票
✅保存cookie 
🔲自动滑块发送验证码
🔲定时抢票
🔲自动选择购票人

## 运行

下载本项目后，配置文件夹下 *ticketconfig.json*
```text
{
  "ActivityId": "182990",  --场次id
  "TicketId": "0fc7521c6836ac036a39b1dabace907e", --票种类id
  "TicketNum": "1", --购买张数
  "StartTime": null, --预定抢购时间
  "NeedSelect": false, --是否自动选择购票人
  "SelectNum": 1  --购票人数
}
```

项目文件夹执行 

```bash
  dotnet run
```
    
### 免责声明

本仓库发布的 `xiudong-playwright` 项目中涉及的任何脚本，仅用于测试和学习研究，禁止用于商业用途，不能保证其合法性，准确性，完整性和有效性，请根据情况自行判断。

本项目内所有资源文件，禁止任何公众号、自媒体进行任何形式的转载、发布。

本人对任何脚本问题概不负责，包括但不限于由任何脚本错误导致的任何损失或损害.

间接使用脚本的任何用户，包括但不限于建立VPS或在某些行为违反国家/地区法律或相关法规的情况下进行传播, 本人 对于由此引起的任何隐私泄漏或其他后果概不负责。

请勿将 `xiudong-playwright` 项目的任何内容用于商业或非法目的，否则后果自负。

如果任何单位或个人认为该项目的脚本可能涉嫌侵犯其权利，则应及时通知并提供身份证明，所有权证明，我们将在收到认证文件后删除相关脚本。

以任何方式查看此项目的人或直接或间接使用 `xiudong-playwright` 项目的任何脚本的使用者都应仔细阅读此声明。本人 保留随时更改或补充此免责声明的权利。
一旦使用并复制了任何相关脚本或 `xiudong-playwright` 项目，则视为您已接受此免责声明。

您必须在下载后的24小时内从计算机或手机中完全删除以上内容。

本项目遵循 `MIT License` 协议，如果本特别声明与 `MIT License` 协议有冲突之处，以本特别声明为准。

> 您使用或者复制了本仓库且本人制作的任何代码或项目，则视为已接受此声明，请仔细阅读
您在本声明未发出之时点使用或者复制了本仓库且本人制作的任何代码或项目且此时还在使用，则视为已接受此声明，请仔细阅读
## 思路参考

https://github.com/ronething/xiudong-selenium
