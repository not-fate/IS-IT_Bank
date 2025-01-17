// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var api = {
    get: function (url) {
        return fetch(url)
            .then((response) => {
                if (!response.ok) {
                    return Promise.reject({ status: response.status, statusText: response.statusText })                    
                }
                return response.json();
            });
    },
    post: function (url, data) {
        return fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json;charset=utf-8'
            },
            body: JSON.stringify(data)
        })
            .then((response) => {
                if (!response.ok) {
                    return Promise.reject({ status: response.status, statusText: response.statusText })
                }
                return response.json();
            });
    },
    account: {
        search: function (phone, balanceId) {         
            return api.get(`/api/account/search?phone=${encodeURIComponent(phone)}&balanceId=${encodeURIComponent(balanceId)}`);
        },
        current_user: function () {
            return api.get('/api/account/current_user');
        },
        allusers: function () {
            return api.get('/api/account/allusers');
        },
        edit: function (data) {
            return api.post('/api/account/edit',data);
        }
    },
    transact: {
        create_transact: function (account_to, amount, detail, extern_account) {
            return api.post('/api/transact/create_transact', { account_to: account_to, amount: amount, detail: detail, extern_account: extern_account });
        },
        list: function () {
            return api.get('/api/transact/list');
        }
    }
}

var utils = {
    fill: function (cont, data) {
        if (typeof cont === 'string') cont = document.querySelector(cont);
        cont = cont || document;
        for (var sel in data) {
            var p = sel.indexOf('$');

            var el = cont.querySelector(p > 0 ? sel.substring(0, p) : sel);
            var prop = p > 0 ? sel.substring(p+1) : null;
            var val = data[sel];

            if (prop)
                el[prop] = val;
            else if (el.tagName == 'INPUT')
                el.value = val;
            else el.innerHTML = val;
        }
    },
    clearChilds: function (cont) {
        while (cont.firstChild) {
            cont.removeChild(cont.lastChild);
        }
    }
};

var page = {
    init: function () {       
        page.dom = {
            searchAccount: document.getElementById('searchAccount'),
            receiverName: document.getElementById('receiverName'),
            transferForm: document.querySelector('.transfer_form'),
            transfer_button: document.querySelector('.transfer_button'),
            transfer_amount: document.getElementById('transfer_amount'),

            user_name: document.querySelector('.user_name'),
            user_email: document.querySelector('.user_email'),
            balance_amount: document.querySelector('.balance_amount'),

            transaction_item_template: document.querySelector('#transaction-item')
        }
        page.dom.transaction_list = page.dom.transaction_item_template.parentNode;
        page.dom.transfer_button.onclick = page.create_trans;

        page.load_curr_user();
    },
    load_curr_user: function () {
        api.account.current_user()
            .then(r => {
                page.dom.user_name.innerHTML = `${r.firstname} ${r.lastname} (${r.phone})`;
                page.dom.user_email.innerHTML = r.email;
                page.dom.balance_amount.innerHTML = r.amount.toFixed(2) + "  ₽";
                page.dom.balance_amount.className = "balance_amount " + (r.amount < 0 ? 'balance_outcome' : 'balance_income');

                document.getElementById('isadmin_menu').style.display = r.isadmin ? 'inline' : 'none';
                /*
                                usr.id,
                usr.phone,
                usr.firstname,
                usr.lastname,
                usr.email,
                balance.amount,
                balance.maxcredit
                */
            });
        api.transact.list()
            .then(r => {
                var cont = page.dom.transaction_list;
                utils.clearChilds(cont);
                for (var i = 0; i < r.length; i++) {
                    var item = page.dom.transaction_item_template.content.cloneNode(true);
                    var el = r[i];                    
                    utils.fill(item,
                        {
                            '.transaction-date': el.date,
                            '.transaction-amount': (el.type == 1 ? "+" : "-") + el.amount.toFixed(2),
                            '.transaction-details': (el.type == 1 ? "От:" : "Кому:") + el.partner,
                            'li$className': el.type == 1 ? "income" : "outcome"
                        });
                    //item.className = el.type == 1? "income": "income";
                    cont.appendChild(item);
                }
            });
    },
    account_search: function () {
        var pattern = page.dom.searchAccount.value;
        page.account_to = null;
        var rez = api.account.search(pattern, pattern);
        rez.then(r => {            
            page.account_to = r;
            receiverName.innerHTML = r.name;
            page.dom.transferForm.className = "transfer_form trans_acc_finded";
        })
            .catch(err => {
                if (err.status == 404) { page.dom.transferForm.className = "transfer_form trans_acc_notfound"; }
                console.log(err);
            });
    },
    create_trans: function () {
        if (!page.account_to) { console.log("account_to not selected"); return; }

        var amount = page.dom.transfer_amount.value;

        page.dom.transfer_button.disabled = true;
        api.transact.create_transact(page.account_to.id, amount)
            .then(r => {
                alert("transaction is complete");
                page.dom.transfer_button.disabled = false;
                page.load_curr_user();
            })
            .catch(err => {
                alert(err.status==400?"Too low balance": "transaction is Failed");
                console.log(err.statusText);
                page.dom.transfer_button.disabled = false;
            });

    }
}

var loginPage = {
    init: function () {
        loginPage.dom = {
            login_cont: document.querySelector('.login_cont'),
        };
    },
    show_login: function () { loginPage.dom.login_cont.className = "login_cont"; },
    show_register: function () { loginPage.dom.login_cont.className = "login_cont_register"; }
}

var adminPage = {
    init: function () {
        adminPage.dom = {
            table_tbody: document.querySelector('.table-container tbody'),
            row_template: document.querySelector('#row_template')
        };
        adminPage.load_users();
    },
    load_users: function () {
        api.account.allusers()
            .then(r => {
                utils.clearChilds(adminPage.dom.table_tbody);

                for (var i = 0; i < r.length; i++) {
                    var el = r[i];
                    const id = el.id;
                    var item = adminPage.dom.row_template.content.cloneNode(true);                   
                    utils.fill(item, {
                        '.phone': el.phone,
                        '.firstname': el.firstname,
                        '.lastname': el.lastname,
                        '.email': el.email,
                        '.amount': el.amount.toFixed(2),
                        '.maxcredit': el.maxcredit,
                        '.maxcredit$onchange': function () {
                            api.account.edit({ id: id, maxcredit: this.value.replace(',', '.') });                            
                        },
                        '.isadmin$onchange': function () {
                            api.account.edit({ id: id, isadmin: this.checked });
                        },
                        '.isadmin$checked':el.isadmin
                    });
                    adminPage.dom.table_tbody.appendChild(item);
                }
            });
    }
};
