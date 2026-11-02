// Product Catalog Client Application
(function () {
    "use strict";

    const STORAGE_KEY_THEME = "catalog_theme";

    // State
    let activeFilter = false;
    let deletingProductId = null;

    // DOM Elements
    const themeToggleBtn = document.getElementById("theme-toggle");
    const themeIcon = document.getElementById("theme-icon");

    const catalogSection = document.getElementById("catalog-section");
    const filterForm = document.getElementById("filter-form");
    const filterNameInput = document.getElementById("filter-name");
    const filterMinPriceInput = document.getElementById("filter-min-price");
    const filterMaxPriceInput = document.getElementById("filter-max-price");
    const btnResetFilter = document.getElementById("btn-reset-filter");
    const filterValidationAlert = document.getElementById("filter-validation-alert");

    const catalogLoading = document.getElementById("catalog-loading");
    const catalogError = document.getElementById("catalog-error");
    const catalogErrorMessage = document.getElementById("catalog-error-message");
    const btnRetryFetch = document.getElementById("btn-retry-fetch");
    const catalogEmpty = document.getElementById("catalog-empty");
    const emptyStateText = document.getElementById("empty-state-text");
    const btnEmptyClearFilter = document.getElementById("btn-empty-clear-filter");
    const btnEmptyAdd = document.getElementById("btn-empty-add");

    const tableWrapper = document.getElementById("table-wrapper");
    const productsTableBody = document.getElementById("products-table-body");

    // Add / Edit Dialog Elements
    const productDialog = document.getElementById("product-dialog");
    const dialogTitle = document.getElementById("dialog-title");
    const productForm = document.getElementById("product-form");
    const productIdInput = document.getElementById("product-id");
    const productNameInput = document.getElementById("product-name");
    const productDescriptionInput = document.getElementById("product-description");
    const productPriceInput = document.getElementById("product-price");
    const productFormError = document.getElementById("product-form-error");
    const productNameError = document.getElementById("product-name-error");
    const productDescriptionError = document.getElementById("product-description-error");
    const productPriceError = document.getElementById("product-price-error");
    const btnOpenCreate = document.getElementById("btn-open-create");
    const btnDialogCancel = document.getElementById("btn-dialog-cancel");
    const btnDialogCloseX = document.getElementById("btn-dialog-close-x");

    // Delete Dialog Elements
    const deleteDialog = document.getElementById("delete-dialog");
    const deleteProductName = document.getElementById("delete-product-name");
    const deleteDialogError = document.getElementById("delete-dialog-error");
    const btnDeleteCancel = document.getElementById("btn-delete-cancel");
    const btnDeleteCloseX = document.getElementById("btn-delete-close-x");
    const btnDeleteConfirm = document.getElementById("btn-delete-confirm");

    // =========================================================================
    // Theme Management
    // =========================================================================
    function updateThemeButtonLabel() {
        const isDark = document.documentElement.getAttribute("data-theme") === "dark";
        if (themeIcon) {
            themeIcon.textContent = isDark ? "Light Mode" : "Dark Mode";
        }
    }

    if (themeToggleBtn) {
        updateThemeButtonLabel();
        themeToggleBtn.addEventListener("click", function () {
            const currentTheme = document.documentElement.getAttribute("data-theme");
            const newTheme = currentTheme === "dark" ? "light" : "dark";
            document.documentElement.setAttribute("data-theme", newTheme);
            localStorage.setItem(STORAGE_KEY_THEME, newTheme);
            updateThemeButtonLabel();
        });
    }

    // =========================================================================
    // API Requests Helper
    // =========================================================================
    async function apiFetch(url, options = {}) {
        return fetch(url, options);
    }

    // =========================================================================
    // Products Fetching and Rendering
    // =========================================================================
    function setCatalogState(state) {
        catalogLoading.style.display = state === "loading" ? "block" : "none";
        catalogError.style.display = state === "error" ? "block" : "none";
        catalogEmpty.style.display = state === "empty" ? "block" : "none";
        tableWrapper.style.display = state === "table" ? "block" : "none";
    }

    async function fetchProducts(searchUrl = "/api/products") {
        setCatalogState("loading");

        try {
            const response = await apiFetch(searchUrl);
            if (!response.ok) {
                const errorData = await response.json().catch(() => ({}));
                throw new Error(errorData.message || "Failed to load products from server.");
            }

            const products = await response.json();
            renderProducts(products);
        } catch (err) {
            catalogErrorMessage.textContent = err.message || "An unexpected error occurred.";
            setCatalogState("error");
        }
    }

    function renderProducts(products) {
        productsTableBody.replaceChildren();

        if (!Array.isArray(products) || products.length === 0) {
            if (activeFilter) {
                emptyStateText.textContent = "No products found matching your current filter.";
                btnEmptyClearFilter.style.display = "inline-flex";
            } else {
                emptyStateText.textContent = "The catalog currently has no products.";
                btnEmptyClearFilter.style.display = "none";
            }
            setCatalogState("empty");
            return;
        }

        products.forEach(function (product) {
            const tr = document.createElement("tr");

            // ID
            const tdId = document.createElement("td");
            tdId.className = "col-id";
            tdId.setAttribute("data-label", "ID");
            tdId.textContent = product.id;
            tr.appendChild(tdId);

            // Name
            const tdName = document.createElement("td");
            tdName.className = "col-name";
            tdName.setAttribute("data-label", "Name");
            tdName.textContent = product.name;
            tr.appendChild(tdName);

            // Description
            const tdDesc = document.createElement("td");
            tdDesc.className = "col-desc";
            tdDesc.setAttribute("data-label", "Description");
            tdDesc.textContent = product.description;
            tr.appendChild(tdDesc);

            // Price (Indonesian Rupiah formatting)
            const tdPrice = document.createElement("td");
            tdPrice.className = "col-price";
            tdPrice.setAttribute("data-label", "Price");
            tdPrice.textContent = new Intl.NumberFormat("id-ID", {
                style: "currency",
                currency: "IDR",
                maximumFractionDigits: 0
            }).format(product.price);
            tr.appendChild(tdPrice);

            // Created At
            const tdDate = document.createElement("td");
            tdDate.className = "col-date";
            tdDate.setAttribute("data-label", "Created At");
            tdDate.textContent = new Date(product.createdAt).toLocaleDateString(undefined, {
                year: "numeric",
                month: "short",
                day: "numeric"
            });
            tr.appendChild(tdDate);

            // Actions
            const tdActions = document.createElement("td");
            tdActions.className = "col-actions";
            tdActions.setAttribute("data-label", "Actions");

            const actionsDiv = document.createElement("div");
            actionsDiv.className = "row-actions";

            const btnEdit = document.createElement("button");
            btnEdit.type = "button";
            btnEdit.className = "btn btn-secondary btn-sm";
            btnEdit.textContent = "Edit";
            btnEdit.setAttribute("aria-label", "Edit " + product.name);
            btnEdit.addEventListener("click", function () {
                openEditDialog(product);
            });

            const btnDelete = document.createElement("button");
            btnDelete.type = "button";
            btnDelete.className = "btn btn-danger btn-sm";
            btnDelete.textContent = "Delete";
            btnDelete.setAttribute("aria-label", "Delete " + product.name);
            btnDelete.addEventListener("click", function () {
                openDeleteDialog(product);
            });

            actionsDiv.appendChild(btnEdit);
            actionsDiv.appendChild(btnDelete);
            tdActions.appendChild(actionsDiv);
            tr.appendChild(tdActions);

            productsTableBody.appendChild(tr);
        });

        setCatalogState("table");
    }

    if (btnRetryFetch) {
        btnRetryFetch.addEventListener("click", function () {
            fetchProducts();
        });
    }

    // =========================================================================
    // Search & Filtering
    // =========================================================================
    if (filterForm) {
        filterForm.addEventListener("submit", function (e) {
            e.preventDefault();
            filterValidationAlert.style.display = "none";
            filterValidationAlert.textContent = "";

            const name = filterNameInput.value.trim();
            const minPrice = filterMinPriceInput.value ? parseFloat(filterMinPriceInput.value) : null;
            const maxPrice = filterMaxPriceInput.value ? parseFloat(filterMaxPriceInput.value) : null;

            if (minPrice !== null && maxPrice !== null && minPrice > maxPrice) {
                filterValidationAlert.textContent = "Minimum price cannot be greater than maximum price.";
                filterValidationAlert.style.display = "block";
                return;
            }

            const params = new URLSearchParams();
            if (name) params.append("name", name);
            if (minPrice !== null && !isNaN(minPrice)) params.append("minPrice", minPrice.toString());
            if (maxPrice !== null && !isNaN(maxPrice)) params.append("maxPrice", maxPrice.toString());

            activeFilter = Boolean(name || minPrice !== null || maxPrice !== null);
            fetchProducts("/api/products/search?" + params.toString());
        });
    }

    function resetFilter() {
        filterNameInput.value = "";
        filterMinPriceInput.value = "";
        filterMaxPriceInput.value = "";
        filterValidationAlert.style.display = "none";
        activeFilter = false;
        fetchProducts("/api/products");
    }

    if (btnResetFilter) {
        btnResetFilter.addEventListener("click", resetFilter);
    }

    if (btnEmptyClearFilter) {
        btnEmptyClearFilter.addEventListener("click", resetFilter);
    }

    // =========================================================================
    // Product Add / Edit Modal Dialog
    // =========================================================================
    function clearProductFormErrors() {
        productFormError.style.display = "none";
        productFormError.textContent = "";
        productNameError.textContent = "";
        productDescriptionError.textContent = "";
        productPriceError.textContent = "";
    }

    function openCreateDialog() {
        clearProductFormErrors();
        productIdInput.value = "";
        productNameInput.value = "";
        productDescriptionInput.value = "";
        productPriceInput.value = "";
        dialogTitle.textContent = "Add New Product";
        productDialog.showModal();
        productNameInput.focus();
    }

    function openEditDialog(product) {
        clearProductFormErrors();
        productIdInput.value = product.id;
        productNameInput.value = product.name;
        productDescriptionInput.value = product.description;
        productPriceInput.value = product.price;
        dialogTitle.textContent = "Edit Product";
        productDialog.showModal();
        productNameInput.focus();
    }

    function closeProductDialog() {
        productDialog.close();
        clearProductFormErrors();
    }

    if (btnOpenCreate) {
        btnOpenCreate.addEventListener("click", openCreateDialog);
    }
    if (btnEmptyAdd) {
        btnEmptyAdd.addEventListener("click", openCreateDialog);
    }
    if (btnDialogCancel) {
        btnDialogCancel.addEventListener("click", closeProductDialog);
    }
    if (btnDialogCloseX) {
        btnDialogCloseX.addEventListener("click", closeProductDialog);
    }

    if (productForm) {
        productForm.addEventListener("submit", async function (e) {
            e.preventDefault();
            clearProductFormErrors();

            const id = productIdInput.value ? parseInt(productIdInput.value, 10) : null;
            const name = productNameInput.value.trim();
            const description = productDescriptionInput.value.trim();
            const price = parseFloat(productPriceInput.value);

            let hasError = false;
            if (!name) {
                productNameError.textContent = "Product name is required.";
                hasError = true;
            } else if (name.length < 2 || name.length > 120) {
                productNameError.textContent = "Name must be between 2 and 120 characters.";
                hasError = true;
            }

            if (!description) {
                productDescriptionError.textContent = "Description is required.";
                hasError = true;
            } else if (description.length < 3 || description.length > 1000) {
                productDescriptionError.textContent = "Description must be between 3 and 1000 characters.";
                hasError = true;
            }

            if (isNaN(price) || price <= 0) {
                productPriceError.textContent = "Price must be a positive number greater than 0.";
                hasError = true;
            }

            if (hasError) return;

            const isEdit = Boolean(id);
            const url = isEdit ? "/api/products/" + id : "/api/products";
            const method = isEdit ? "PUT" : "POST";

            try {
                const response = await apiFetch(url, {
                    method: method,
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ name, description, price })
                });

                if (response.ok) {
                    closeProductDialog();
                    fetchProducts(activeFilter ? undefined : "/api/products");
                } else {
                    const errData = await response.json().catch(() => ({}));
                    if (errData.errors) {
                        // ASP.NET Core Validation Problem Details
                        const messages = Object.values(errData.errors).flat();
                        productFormError.textContent = messages.join(" ");
                    } else {
                        productFormError.textContent = errData.message || "Failed to save product.";
                    }
                    productFormError.style.display = "block";
                }
            } catch (err) {
                productFormError.textContent = err.message || "Network error while saving product.";
                productFormError.style.display = "block";
            }
        });
    }

    // =========================================================================
    // Product Delete Dialog
    // =========================================================================
    function openDeleteDialog(product) {
        deletingProductId = product.id;
        deleteProductName.textContent = product.name;
        deleteDialogError.style.display = "none";
        deleteDialogError.textContent = "";
        deleteDialog.showModal();
        btnDeleteCancel.focus();
    }

    function closeDeleteDialog() {
        deleteDialog.close();
        deletingProductId = null;
    }

    if (btnDeleteCancel) {
        btnDeleteCancel.addEventListener("click", closeDeleteDialog);
    }
    if (btnDeleteCloseX) {
        btnDeleteCloseX.addEventListener("click", closeDeleteDialog);
    }

    if (btnDeleteConfirm) {
        btnDeleteConfirm.addEventListener("click", async function () {
            if (!deletingProductId) return;

            btnDeleteConfirm.disabled = true;
            btnDeleteConfirm.textContent = "Deleting...";

            try {
                const response = await apiFetch("/api/products/" + deletingProductId, {
                    method: "DELETE"
                });

                if (response.ok) {
                    closeDeleteDialog();
                    fetchProducts();
                } else {
                    const errData = await response.json().catch(() => ({}));
                    deleteDialogError.textContent = errData.message || "Failed to delete product.";
                    deleteDialogError.style.display = "block";
                }
            } catch (err) {
                deleteDialogError.textContent = err.message || "Network error while deleting product.";
                deleteDialogError.style.display = "block";
            } finally {
                btnDeleteConfirm.disabled = false;
                btnDeleteConfirm.textContent = "Delete";
            }
        });
    }

    // Initialize catalog on page load
    fetchProducts();
})();
