"Use strict"

var AddOrderLineTable = {

    createOrderLineTable: function (orderNumber) {
        var row = document.getElementById(orderNumber);

        var innerTable = document.createElement('table');
        innerTable.id = 'Order-line-table_' + orderNumber;
        innerTable.className = 'table table-striped table-bordered table-hover';
        var headerRow = document.createElement('thead');

        var tableRow = document.createElement('tr');
        tableRow.className = 'col-md-12'
        var tBody = document.createElement('tbody');

        var rowCell1 = document.createElement('th');
        rowCell1.className = 'col-md-2';
        var rowCell2 = document.createElement('th');
        rowCell2.className = 'col-md-2';
        var rowCell3 = document.createElement('th');
        rowCell3.className = 'col-md-2';
        var rowCell4 = document.createElement('th');
        rowCell4.className = 'col-md-2';
        var rowCell5 = document.createElement('th');
        rowCell5.className = 'col-md-2';
        var rowCell6 = document.createElement('th');
        rowCell6.className = 'col-md-2';

        rowCell1.textContent = 'Product Code';
        rowCell2.textContent = 'Product Type';
        rowCell3.textContent = 'Cost Price';
        rowCell4.textContent = 'Sales Price';
        rowCell5.textContent = 'Quantity';
        rowCell6.textContent = '';

        tableRow.appendChild(rowCell1);
        tableRow.appendChild(rowCell2);
        tableRow.appendChild(rowCell3);
        tableRow.appendChild(rowCell4);
        tableRow.appendChild(rowCell5);
        tableRow.appendChild(rowCell6);

        headerRow.appendChild(tableRow);

        innerTable.appendChild(headerRow);
        innerTable.appendChild(tBody);
        AddOrderLineTable.populateOrderLineTable(orderNumber);

        return innerTable;
    },

    populateOrderLineTable: function (orderNumber) {
        $.ajax({
            url: '/OrderLine/GetOrderLines', // Update the URL to your actual endpoint
            type: 'GET',
            data: { OrderNumber: orderNumber },
            success: function (data) {
                
                var orderLineTableBody = $('#Order-line-table_' + orderNumber + ' tbody');
                orderLineTableBody.empty(); // Clear any existing rows
                console.log(data);
                // Iterate through the data and append rows to the table body
                data.forEach(function (orderLine) {
                    var row = `
                    <tr class="order-line-row" data-order-id="${orderNumber}" data-line-id="${orderLine.lineNumber}" id="OrderLine_${orderLine.lineNumber}">
                        <td>${orderLine.productCode}</td>
                        <td>${orderLine.productType}</td>
                        <td>${orderLine.costPrice}</td>
                        <td>${orderLine.salesPrice}</td>
                        <td>${orderLine.quantity}</td>
                        <td>
                            <button class="btn btn-warning btn-sm edit-order-line-btn" data-line-id="${orderLine.lineNumber}">Edit</button>
                            <button class="btn btn-danger btn-sm delete-order-line-btn" data-line-id="${orderLine.lineNumber}">Remove</button>
                        </td>
                    </tr>
                `;
                    orderLineTableBody.append(row);
                });

                $('.edit-order-line-btn').on('click', function () {
                    var lineId = $(this).data('line-id');
                    $('#orderLineModal .modal-content').load(`/OrderLine/EditOrderLine?orderId=${orderNumber}&lineId=${lineId}`);
                    $('#orderLineModal').modal('show');
                });

                $('.delete-order-line-btn').on('click', function () {
                    var lineId = $(this).data('line-id');
                    if (confirm('Are you sure you want to delete this order line?')) {
                        $.ajax({
                            url: `/OrderLine/DeleteOrderLine/${lineId}`,
                            type: 'POST',
                            success: function () {
                                $('#OrderLine_' + lineId).remove();
                            }
                        });
                    }
                });
            },
            error: function (error) {
                console.error('Error fetching order lines:', error);
            }
        });
},


    populateEditOrderLineModal: function (model) {
        let modalBody = document.querySelector('#editOrderLineModal .modal-body');
        if (modalBody == null) {
            alert("Error: Modal body not found");
            return;
        }

        modalBody.innerHTML = '';

        // Create form element
        let formElement = document.createElement('form');
        formElement.id = 'editOrderLineForm';
        formElement.method = 'post';
        formElement.action = '/OrderLine/EditOrderLine';

        // Create hidden order number input
        let orderNumberInput = document.createElement('input');
        orderNumberInput.type = 'hidden';
        orderNumberInput.name = 'OrderNumber';
        orderNumberInput.id = 'OrderNumber';
        orderNumberInput.value = model.orderNumber;
        formElement.appendChild(orderNumberInput);

        // Create hidden Line Number row
        let lineNumberRow = document.createElement('div');
        lineNumberRow.className = "form-group row";

        let lineNumberLabelCol = document.createElement('div');
        lineNumberLabelCol.className = "col-md-6";

        let lineNumberLabel = document.createElement('label');
        lineNumberLabel.innerText = "Line Number";
        lineNumberLabel.className = "control-label";
        lineNumberLabelCol.appendChild(lineNumberLabel);

        let lineNumberInputCol = document.createElement('div');
        lineNumberInputCol.className = "col-md-6";

        let lineNumberInput = document.createElement('input');
        lineNumberInput.type = "hidden";
        lineNumberInput.className = "form-control";
        lineNumberInput.name = "LineNumber";
        lineNumberInput.id = "LineNumber";
        lineNumberInput.value = model.lineNumber;
        lineNumberInputCol.appendChild(lineNumberInput);

        lineNumberRow.appendChild(lineNumberLabelCol);
        lineNumberRow.appendChild(lineNumberInputCol);

        // formElement.appendChild(lineNumberRow);

        // Create Product Code row
        let productCodeRow = document.createElement('div');
        productCodeRow.className = "form-group row";

        let productCodeLabelCol = document.createElement('div');
        productCodeLabelCol.className = "col-md-6";

        let productCodeLabel = document.createElement('label');
        productCodeLabel.innerText = "Product Code";
        productCodeLabel.className = "control-label";
        productCodeLabelCol.appendChild(productCodeLabel);

        let productCodeInputCol = document.createElement('div');
        productCodeInputCol.className = "col-md-6";

        let productCodeInput = document.createElement('input');
        productCodeInput.type = "text";
        productCodeInput.className = "form-control";
        productCodeInput.name = "ProductCode";
        productCodeInput.id = "ProductCode";
        productCodeInput.value = model.productCode;
        productCodeInputCol.appendChild(productCodeInput);

        productCodeRow.appendChild(productCodeLabelCol);
        productCodeRow.appendChild(productCodeInputCol);

        formElement.appendChild(productCodeRow);

        // Create Product Type row with dropdown
        let productTypeRow = document.createElement('div');
        productTypeRow.className = "form-group row";

        let productTypeLabelCol = document.createElement('div');
        productTypeLabelCol.className = "col-md-6";

        let productTypeLabel = document.createElement('label');
        productTypeLabel.innerText = "Product Type";
        productTypeLabel.className = "control-label";
        productTypeLabelCol.appendChild(productTypeLabel);

        let productTypeSelectCol = document.createElement('div');
        productTypeSelectCol.className = "col-md-6";

        let productTypeSelect = document.createElement('select');
        productTypeSelect.name = "ProductType";
        productTypeSelect.id = "ProductType";
        productTypeSelect.className = "form-control";

        let productTypes = ["Apparel", "Parts", "Equipment", "Motor"];
        productTypes.forEach(type => {
            let option = document.createElement('option');
            option.value = type;
            option.innerText = type;
            if (type === model.productType) {
                option.selected = true;
            }
            productTypeSelect.appendChild(option);
        });

        productTypeSelectCol.appendChild(productTypeSelect);
        productTypeRow.appendChild(productTypeLabelCol);
        productTypeRow.appendChild(productTypeSelectCol);

        formElement.appendChild(productTypeRow);

        // Create Cost Price row
        let costPriceRow = document.createElement('div');
        costPriceRow.className = "form-group row";

        let costPriceLabelCol = document.createElement('div');
        costPriceLabelCol.className = "col-md-6";

        let costPriceLabel = document.createElement('label');
        costPriceLabel.innerText = "Cost Price";
        costPriceLabel.className = "control-label";
        costPriceLabelCol.appendChild(costPriceLabel);

        let costPriceInputCol = document.createElement('div');
        costPriceInputCol.className = "col-md-6";

        let costPriceInput = document.createElement('input');
        costPriceInput.type = "text";
        costPriceInput.className = "form-control";
        costPriceInput.name = "CostPrice";
        costPriceInput.id = "CostPrice";
        costPriceInput.value = model.costPrice;
        costPriceInputCol.appendChild(costPriceInput);

        costPriceRow.appendChild(costPriceLabelCol);
        costPriceRow.appendChild(costPriceInputCol);

        formElement.appendChild(costPriceRow);

        // Create Sales Price row
        let salesPriceRow = document.createElement('div');
        salesPriceRow.className = "form-group row";

        let salesPriceLabelCol = document.createElement('div');
        salesPriceLabelCol.className = "col-md-6";

        let salesPriceLabel = document.createElement('label');
        salesPriceLabel.innerText = "Sales Price";
        salesPriceLabel.className = "control-label";
        salesPriceLabelCol.appendChild(salesPriceLabel);

        let salesPriceInputCol = document.createElement('div');
        salesPriceInputCol.className = "col-md-6";

        let salesPriceInput = document.createElement('input');
        salesPriceInput.type = "text";
        salesPriceInput.className = "form-control";
        salesPriceInput.name = "SalesPrice";
        salesPriceInput.id = "SalesPrice";
        salesPriceInput.value = model.salesPrice;
        salesPriceInputCol.appendChild(salesPriceInput);

        salesPriceRow.appendChild(salesPriceLabelCol);
        salesPriceRow.appendChild(salesPriceInputCol);

        formElement.appendChild(salesPriceRow);

        // Create Quantity row
        let quantityRow = document.createElement('div');
        quantityRow.className = "form-group row";

        let quantityLabelCol = document.createElement('div');
        quantityLabelCol.className = "col-md-6";

        let quantityLabel = document.createElement('label');
        quantityLabel.innerText = "Quantity";
        quantityLabel.className = "control-label";
        quantityLabelCol.appendChild(quantityLabel);

        let quantityInputCol = document.createElement('div');
        quantityInputCol.className = "col-md-6";

        let quantityInput = document.createElement('input');
        quantityInput.type = "text";
        quantityInput.className = "form-control";
        quantityInput.name = "Quantity";
        quantityInput.id = "Quantity";
        quantityInput.value = model.quantity;
        quantityInputCol.appendChild(quantityInput);

        quantityRow.appendChild(quantityLabelCol);
        quantityRow.appendChild(quantityInputCol);

        formElement.appendChild(quantityRow);

        // Append form to modal body
        modalBody.appendChild(formElement);

        // Show the modal
        $('#editOrderLineModal').modal('show');
    },

}