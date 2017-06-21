$(document).ready(function () {
    BindingDateTime();
});

function _bindSingle(dom) {
    var time = parseInt(dom.attr('datetime'));
    dom.html(new Date(time - new Date().getTimezoneOffset() * 60 * 1000));
    var now = new Date().getTime() - new Date().getTimezoneOffset() * 60 * 1000;
    var span = now - time;
    if (span == 0) {
        if (dom.html() != '现在')
            dom.html('现在');
    }
    else if (span > 0) {
        var _span = span / 1000;
        if (_span < 60) {
            if (dom.html() != parseInt(_span) + '秒前')
                dom.html(parseInt(_span) + '秒前');
            return;
        }
        _span = _span / 60;
        if (_span < 60) {
            if (dom.html() != parseInt(_span) + '分钟前')
                dom.html(parseInt(_span) + '分钟前');
            return;
        }
        _span = _span / 60;
        if (_span < 24) {
            if (dom.html() != parseInt(_span) + '小时前')
                dom.html(parseInt(_span) + '小时前');
            return;
        }
        _span = _span / 24;
        if (_span < 30) {
            if (dom.html() != parseInt(_span) + '天前')
                dom.html(parseInt(_span) + '天前');
            return;
        }
        _span = _span / 30;
        if (_span < 12) {
            if (dom.html() != parseInt(_span) + '个月前')
                dom.html(parseInt(_span) + '个月前');
            return;
        }
        _span = _span / 12;
        if (dom.html() != parseInt(_span) + '年前')
            dom.html(parseInt(_span) + '年前');
        return;
    }
    else {
        var _span = -span;
        _span = _span / 1000;
        if (_span < 60) {
            if (dom.html() != parseInt(_span) + '秒秒后前')
            dom.html(parseInt(_span) + '秒后');
            return;
        }
        _span = _span / 60;
        if (_span < 60) {
            if (dom.html() != parseInt(_span) + '分钟后')
            dom.html(parseInt(_span) + '分钟后');
            return;
        }
        _span = _span / 60;
        if (_span < 24) {
            if (dom.html() != parseInt(_span) + '小时后')
            dom.html(parseInt(_span) + '小时后');
            return;
        }
        _span = _span / 24;
        if (_span < 30) {
            if (dom.html() != parseInt(_span) + '天后')
            dom.html(parseInt(_span) + '天后');
            return;
        }
        _span = _span / 30;
        if (_span < 12) {
            if (dom.html() != parseInt(_span) + '个月后')
            dom.html(parseInt(_span) + '个月后');
            return;
        }
        _span = _span / 12;
        if (dom.html() != parseInt(_span) + '年后')
        dom.html(parseInt(_span) + '年后');
        return;
    }

}

function BindingDateTime() {
    setInterval(function () {
        var datetime = $('[datetime]');
        for (var i = 0; i < datetime.length; i++) {
            _bindSingle($(datetime[i]));
        }
    }, 1000);
}