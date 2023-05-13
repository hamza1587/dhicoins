var datatable;
$(document).ready(function () {

    loadDataTable();
});
function loadDataTable() {
    var i = 1;
    datatable = $('#Currencies_Master').DataTable({

        "ajax": {
            "url": "/Admin/GetCurrenciesMaster",
            "type": "GET",
            "datatype": "json",
            "cache": false
        },
        "columns": [
            {
                "render": function (data, type, full, meta) {
                    return i++;
                }
            },

            { "data": "currencyName", "autoWidth": true },
            { "data": "symbol", "autoWidth": true },
            {
                "data": "buyStatus",
                "autoWidth": true,
                "render": function (data, type, row, meta) {
                    if (data === "Inactive") {
                        return '<span class="kt-badge kt-badge--inline kt-badge--danger">Inactive</span>';
                    } else if (data === "Active") {
                        return '<span class="kt-badge kt-badge--inline kt-badge--success">Active</span>';
                    } else {
                        return data;
                    }
                }
            },
            {
                "data": "sellStatus",
                "autoWidth": true,
                "render": function (data, type, row, meta) {
                    if (data === "Inactive") {
                        return '<span class="kt-badge kt-badge--inline kt-badge--danger">Inactive</span>';
                    } else if (data === "Active") {
                        return '<span class="kt-badge kt-badge--inline kt-badge--success">Active</span>';
                    } else {
                        return data;
                    }
                }
            },

            {
                "data": "id",
                "render": function (data) {
                    return `
                    <a  href="javascript:void(0)" onclick="Edit('${data}')" title="Edit" >
                          <i class="fas fa-edit"></i></a>
                    <a href="javascript:void(0)" onclick="DeleteCurrency('${data}')" title="Delete">
                    <i class="fas fa-trash"></i>
                    </a>
                    
`
                },
                "width": "5%"
            }

        ]
    });
}

$("#btnOpen").click(function () {
    $('#form')[0].reset();
    $("#AddNewCurrency").modal("show");
    $("#exampleModalLabel").text('Add New Currency');
    $("#btnSave").text('Add New');
})


$("#btnSave").click(function () {

    $("#CurrencyNameErr").text('');
    $("#SymbolErr").text('');
    $("#CoinIdErr").text('');
    $("#BuyCommissionErr").text('');
    $("#SellCommissionErr").text('');
    $("#BuyLimitErr").text('');
    $("#SellLimitErr").text('');
    $("#BuyConversionRateErr").text('');
    $("#SellConversionRateErr").text('');
    $("#MaxBuyLimitErr").text('');
    $("#MaxSellLimitErr").text('');
    $("#BuydigitErr").text('');

    if ($("[name='CurrencyName']").val() == "") {
        $("#CurrencyNameErr").text('Currency Name is required!');
    }
    else if ($("[name='Buydigit']").val() == "") {
        $("#BuydigitErr").text('Curreny Digit is required!');
    }
    else if ($("[name='Symbol']").val() == "") {
        $("#SymbolErr").text('Currency Symbol is required!');
    }
    else if ($("[name='CoinId']").val() == "") {
        $("#CoinIdErr").text('Coin Id is required!');
    }
    else if ($("[name='BuyCommission']").val() == "") {
        $("#BuyCommissionErr").text(' Buy Commission in % required!');
    }
    else if ($("[name='SellCommission']").val() == "") {
        $("#SellCommissionErr").text('Sell Commission in % required!');
    }
    else if ($("[name='BuyLimit']").val() == "") {
        $("#BuyLimitErr").text('Buy Limit is required!');
    }
    else if ($("[name='SellLimit']").val() == "") {
        $("#SellLimitErr").text('Sell Limit is required!');
    }
    else if ($("[name='BuyConversionRate']").val() == "") {
        $("#BuyConversionRateErr").text('Buy Conversion Rate in % is required!');
    }
    else if ($("[name='SellConversionRate']").val() == "") {
        $("#SellConversionRateErr").text('Sell Conversion Rate in % is required!');
    }
    else if ($("[name='MaxBuyLimit']").val() == "") {
        $("#MaxBuyLimitErr").text('Max Buy Limit is required!');
    }
    else if ($("[name='MaxSellLimit']").val() == "") {
        $("#MaxSellLimitErr").text('Max Sell Limit is required!');
    }
    else $.ajax({
        url: "/Admin/CurrenciesMaster",
        method: "Post",
        data: $('#form').serialize(),
        datatype: "json",
        success: function (result) {
            datatable.ajax.reload();
            $("#AddNewCurrency").modal("hide");
            $('#form')[0].reset();
        },
        error: function () {
            alert("An error occurred while saving the currency.");
        }
    })

})


function Edit(id) {
    $.ajax({
        url: "/Admin/EditCurrenciesMaster/" + id,
        method: "Get",
        dataType: "json",
        success: function (result) {
           
            $("#exampleModalLabel").text('Edit Currency');

            $('[name="Id"]').val(result.data.id);
            $('[name="CurrencyName"]').val(result.data.currencyName);
            $('[name="Symbol"]').val(result.data.symbol);
            $('[name="CoinId"]').val(result.data.coinId);

            var commissionApplicable = result.data.commissionApplicable;
            $('[name="CommissionApplicable"]').val(commissionApplicable);
            var selectedOption = $('[name="CommissionApplicable"] option[value="' + commissionApplicable + '"]');

            selectedOption.prop('selected', true);
           
            $('[name="BuyCommission"]').val(result.data.buyCommission);
            $('[name="SellCommission"]').val(result.data.sellCommission);

            var buyStatus = result.data.buyStatus;
            $('[name="BuyStatus"]').prop('checked', buyStatus);

            var sellStatus = result.data.sellStatus;
            $('[name="SellStatus"]').prop('checked', sellStatus);

            $('[name="BuyLimit"]').val(result.data.buyLimit);
            $('[name="SellLimit"]').val(result.data.sellLimit);
            $('[name="MaxBuyLimit"]').val(result.data.maxBuyLimit);
            $('[name="MaxSellLimit"]').val(result.data.maxSellLimit);
            $('[name="BuyConversionRate"]').val(result.data.buyConversionRate);
            $('[name="SellConversionRate"]').val(result.data.sellConversionRate);
            $('[name="Buydigit"]').val(result.data.buydigit);
        
            // Populate the selected sell and buy chain IDs

            var selectedBuyChains = result.data.buyChainId;
            $('[name="BuyChainId"]').val(selectedBuyChains);
            for (let i = 0; i < selectedBuyChains.length; i++) {
                $("#BuyChainId option[value='" + selectedBuyChains[i] + "']").prop("selected", true);
            }
            var selectedSellChains = result.data.sellChainId;
            $('[name="SellChainId"]').val(selectedSellChains);
            for (let i = 0; i < selectedSellChains.length; i++) {
                $("#SellChainId option[value='" + selectedSellChains[i] + "']").prop("selected", true);
            }
            $('#AddNewCurrency').modal('show');
           
            $("#btnSave").text('Edit Currency');
        },
        error: function () {
            alert("An error occurred while updating the Currency.");
        }
    })
}

function DeleteCurrency(id) {
    swal({
        title: "Are you sure?",
        text: "You are about to delete this Currency. This action cannot be undone.",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            // Perform the delete action
            $.ajax({
                url: "/Admin/DeleteCurrenciesMaster/" + id,
                method: "GET",
                dataType: "json",
                success: function (result) {
                    swal("Currency deleted!", "The currency has been deleted.", "success");
                    datatable.ajax.reload();
                },
                error: function () {
                    swal("Error", "An error occurred while deleting the currency.", "error");
                }
            });
        }
    });
}


//$('[name="BuyChainId"]').val(result.data.buyChainId);
//$('[name="SellChainId"]').val(result.data.sellChainId);











