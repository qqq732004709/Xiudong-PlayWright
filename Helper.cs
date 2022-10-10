using System;
using Microsoft.Playwright;

namespace PlaywrightDemo;

public class Helper
{
    private string _event;
    private string _ticketId;
    private string _ticketNum;
    private DateTime? _startTime;
    private bool _needSelect;
    private int _selectNum;

    public Helper(string ticketId,
                  string ticketEvent,
                  bool needSelect,
                  DateTime? startTime,
                  string ticketNum = "1",
                  int selectNum = 1)
    {
        _event = ticketEvent;
        _ticketId = ticketId;
        _ticketNum = ticketNum;
        _selectNum = selectNum;
        _startTime = startTime;
        _needSelect = needSelect;
    }

    public async Task LoadTicketPage(IPage page)
    {
        var confirmUrl = $@"https://wap.showstart.com/pages/order/activity/confirm/confirm?sequence="+
            $@"{_event}&ticketId={_ticketId}&ticketNum={_ticketNum}&ioswx=1&terminal=app&from=singlemessage&isappinstalled=0";

        for (int i = 0; i < 3; i++)
        {
            await page.WaitForTimeoutAsync(1000 * 10);
            if (await page.TitleAsync() == "我的")
            {
                Console.WriteLine("登陆成功");
                break;
            }
        }

        if (await page.TitleAsync() != "我的")
        {
            Console.WriteLine("未登录");
            return;
        }

        await page.GotoAsync(confirmUrl);

        await page.WaitForTimeoutAsync(1000 * 100);
    }
}