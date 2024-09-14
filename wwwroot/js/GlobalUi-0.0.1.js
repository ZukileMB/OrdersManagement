"Use strict"
let GlobalCalls = {

    addNewOrderLine: function (orderNumber, lineNumber = 0) {
        $.ajax({
            url: `/OrderLine/GetOrderLinePartial?orderId=${orderNumber}&lineNumber=${lineNumber}`,
            type: 'GET',
            success: function (result) {
                $('#addOrderLineModal .modal-content').html(result);
                $('#addOrderLineModal').modal('show');
            }
        });
    },

    SHowHideOrderLinesByOrderId: function (orderNumber) {

        let element = document.getElementById('order-lines-' + orderNumber);
        if (element.style.display === 'none') {
            element.style.display = 'table-row';
        } else {
            element.style.display = 'none';
        }
    },

    SaveNewOrderLine: function (event) {

        // event.preventDefault();
        // Serialize the form data
        var formData = $('#addOrderLineForm').serializeArray();

        var formDataJson = {}
        $.each(formData, function () {
            formDataJson[this.name] = this.value;
        })

        var url = '/OrderLine/AddOrderLine';
        if (formDataJson.LineNumber != "0") {
            url = '/OrderLine/EditOrderLine'
        }
        GlobalCalls.addProcessingLoader();

        $.ajax({
            url: url,
            type: 'POST',
            data: formData,
            success: function (response) {
                console.log(response);
               GlobalCalls.updateOrderLineTable(formDataJson.OrderNumber);
                $('#addOrderLineModal').modal('hide');
                $(`'#Order-line-table_'${ formDataJson.OrderNumber }`).html(response);
                GlobalCalls.removeProcessingLoader();
            },
            error: function (xhr, status, error) {
                // Handle error
                console.error(error);
                GlobalCalls.removeProcessingLoader();
            }
        });
    },

    addOrderLine: function () {
        let orderLinesTable = document.querySelector("table tbody");
        let rowCount = orderLinesTable.rows.length;

        let row = orderLinesTable.insertRow(rowCount);

        // Create LineNumber cell
        var lineNumberCell = row.insertCell(0);
        var lineNumberInput = document.createElement("input");
        lineNumberInput.name = `OrderLines[${rowCount}].LineNumber`;
        lineNumberInput.className = "form-control";
        lineNumberInput.value = rowCount + 1;
        lineNumberInput.readOnly = true;
        lineNumberCell.appendChild(lineNumberInput);

        // Create ProductCode cell
        var productCodeCell = row.insertCell(1);
        var productCodeInput = document.createElement("input");
        productCodeInput.name = `OrderLines[${rowCount}].ProductCode`;
        productCodeInput.className = "form-control";
        productCodeCell.appendChild(productCodeInput);

        // Create ProductType cell
        var productTypeCell = row.insertCell(2);
        var productTypeSelect = document.createElement("select");
        productTypeSelect.name = `OrderLines[${rowCount}].ProductType`;
        productTypeSelect.className = "form-control";

        var productTypes = ["Apparel", "Parts", "Equipment", "Motor"];
        productTypes.forEach(function (type) {
            var option = document.createElement("option");
            option.value = type;
            option.text = type;
            productTypeSelect.appendChild(option);
        });

        productTypeCell.appendChild(productTypeSelect);

        // Create CostPrice cell
        var costPriceCell = row.insertCell(3);
        var costPriceInput = document.createElement("input");
        costPriceInput.name = `OrderLines[${rowCount}].CostPrice`;
        costPriceInput.className = "form-control";
        costPriceCell.appendChild(costPriceInput);

        // Create SalesPrice cell
        var salesPriceCell = row.insertCell(4);
        var salesPriceInput = document.createElement("input");
        salesPriceInput.name = `OrderLines[${rowCount}].SalesPrice`;
        salesPriceInput.className = "form-control";
        salesPriceCell.appendChild(salesPriceInput);

        // Create Quantity cell
        var quantityCell = row.insertCell(5);
        var quantityInput = document.createElement("input");
        quantityInput.name = `OrderLines[${rowCount}].Quantity`;
        quantityInput.className = "form-control";
        quantityCell.appendChild(quantityInput);

        // Create Remove button cell
        var removeButtonCell = row.insertCell(6);
        var removeButton = document.createElement("button");
        removeButton.type = "button";
        removeButton.className = "btn btn-danger";
        removeButton.onclick = () => GlobalCalls.removeOrderLine(removeButton);
        removeButton.textContent = "Remove";
        removeButtonCell.appendChild(removeButton);
    },

    removeOrderLine: function (button) {
        var row = button.parentNode.parentNode;
        row.parentNode.removeChild(row);
    },


    saveOrderLine: function () {
        var form = $('#editOrderLineForm');
        var data = form.serialize();

        fetch(form.attr('action'), {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: data,
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    $('#editOrderLineModal').modal('hide');
                    window.location.reload(); 
                } else {
                    alert(data.message);
                }
            })
            .catch(error => {
                console.error('Error:', error);
            });

    },

    ShowEditOrderLine: function (orderId, lineId) {

        fetch(`/OrderLine/EditOrderLine?orderId=${orderId}&lineId=${lineId}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok ' + response.statusText);
                }
                return response.json();
            })
            .then(data => {
                if (data.success) {
                    ApplyModalActions.populateEditOrderLineModal(data.data);
                } else {
                    alert(data.message);
                }
            })
            .catch(error => {
                console.error('Error:', error);
                alert('An error occurred while loading the order line data.');
            });
    },

    removeOrder: function (orderId) {
        if (confirm('Deleteng Order will remove the Order lines related to this Order,\n Are you sure you want to delete this order ?'))
        {
            $.ajax({
                url: '/Order/DeleteOrder',
                type: 'POST',
                data: { orderNumber: orderId },
                success: function (response) {
                    GlobalCalls.deleteOrderRow(orderId);
                    window.location.href = window.location.href;
                },
                error: function (xhr, status, error) {
                    console.log(error);
                }
            });
        }
    },

    removeOrderLine: function (orderId, lineId) {
        if (confirm('Are you sure you want to delete this order line?'))
        {
            $.ajax({
                url: '/OrderLine/DeleteOrderLine',
                type: 'POST',
                data: { orderNumber: orderId, lineNumber: lineId },
                success: function (response) {
                    GlobalCalls.deleteLineNumberRow(lineId);
                },
                error: function (xhr, status, error) {
                    console.log(error);
                }
            });
        }
    },

    updateOrderLineTable: function (orderId) {
        $.ajax({
            url: '/OrderLine/GetOrderLines',
            type: 'GET',
            data: { OrderNumber: orderId },
            success: function (response) {
                var tableBody = $('.table tbody');
                tableBody.empty();

                response.forEach(function (orderLine) {
                    var row = `
                    <tr class="order-line-row" data-order-id="${orderLine.OrderNumber}" data-line-id="${orderLine.LineNumber}" id="OrderLine_${orderLine.LineNumber}">
                        <td>${orderLine.ProductCode}</td>
                        <td>${orderLine.ProductType}</td>
                        <td>${orderLine.CostPrice}</td>
                        <td>${orderLine.SalesPrice}</td>
                        <td>${orderLine.Quantity}</td>
                        <td>
                            <button class="btn btn-danger btn-sm" style="margin-right: 10px" onclick="GlobalCalls.removeOrderLine(${orderLine.OrderNumber},${orderLine.LineNumber})">Remove</button>
                        </td>
                    </tr>`;
                    tableBody.append(row);
                });
            },
            error: function (xhr, status, error) {
                console.error(error);
            }
        });
    },

    deleteOrderRow: function (orderNumber) {
        var row = document.getElementById("Order_" + orderNumber);

        if (row) {
            row.remove();
        }
    },

    deleteLineNumberRow: function (lineNumber) {
        var row = document.getElementById("OrderLine_" + lineNumber);

        if (row) {
            row.remove();
        }
    },

    saveEditedOrderLine: function () {

        var orderNumber = document.getElementById('OrderNumber').value;
        var lineNumber = document.getElementById('LineNumber').value;
        var productCode = document.getElementById('ProductCode').value;
        var productType = document.getElementById('ProductType').value;
        var costPrice = document.getElementById('CostPrice').value;
        var salesPrice = document.getElementById('SalesPrice').value;
        var quantity = document.getElementById('Quantity').value;

        let data = {
            OrderNumber: orderNumber,
            LineNumber: lineNumber,
            ProductCode: productCode,
            ProductType: productType,
            CostPrice: costPrice,
            SalesPrice: salesPrice,
            Quantity: quantity
        };

        $.ajax({
            url: '/OrderLine/EditOrderLine',
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json',
            success: function (response) {
                if (response.success) {
                    alert('Order line updated successfully.');

                    $('#editOrderLineModal').modal('hide');
                } else {
                    alert('Error: ' + response.message);
                }
            },
            error: function (xhr, status, error) {
                // Handle error
                console.error(error);
                alert('An error occurred while updating the order line.');
            }
        });
    },

    addProcessingLoader: function () {
        let div = document.getElementById("DisplayAlert");

        let modal = document.getElementById('processingModal');
        let divLoader = document.getElementById('loader');

        let paragraph = modal.querySelector('.modal-body p');

        // check if the paragraph is there else recreate
        // this is done because after the response we remove the paragraph 
        if (!paragraph) {
            let newParagraph = document.createElement('p');
            newParagraph.textContent = 'Please wait while the server is processing your request...';
            newParagraph.style.textAlign = "center";
            modal.querySelector('.modal-body').appendChild(newParagraph);
        }

        $('#processingModal').modal('show');
        divLoader.classList.add("spinner-border");
    },

    removeProcessingLoader: function () {
        let modal = document.getElementById("processingModal");
        //clear the modal response
        let displayAlert = modal.querySelector('#DisplayAlert');
        displayAlert.innerHTML = '';
        displayAlert.classList.remove("alert-danger");
    },

}