// components/ContactForm.tsx
"use client";

import { useEffect, useState } from "react";

import { Field, Formik } from "formik";

export const MainContactForm = () => {
    const [csrfToken, setCsrfToken] = useState<string>("");

    useEffect(() => {
        // Fetch the CSRF token from the server
        const fetchCsrfToken = async () => {
            const response = await fetch("/api/contact");
            const data = await response.json();
            setCsrfToken(data.csrfToken);
        };

        fetchCsrfToken();
    }, []);

    const submitForm = async (formData: any) => {
        const response = await fetch("/api/contact", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "x-csrf-token": csrfToken,
            },
            body: formData,
        });
        const result = await response.json();
        alert(result.message);
    };

    return (
        <div className="container-form-home">
            <h2>Start Your US Expat Tax Return</h2>
            <Formik
                initialValues={{ firstName: "", lastName: "", email: "", issue: "", questionText: "" }}
                validate={(values) => {
                    const errors: any = {};
                    if (!values.firstName) {
                        errors.firstName = "Required";
                    } else if (!values.lastName) {
                        errors.lastName = "Required";
                    } else if (!values.email) {
                        errors.email = "Required";
                    } else if (!/^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i.test(values.email)) {
                        errors.email = "Invalid email address";
                    }
                    return errors;
                }}
                onSubmit={(values, { setSubmitting }) => {
                    setSubmitting(true);

                    const formValues = JSON.stringify(values, null, 2);
                    submitForm(formValues).then(() => {
                        setSubmitting(false);
                    });
                    setTimeout(() => {}, 5000);

                    values.firstName = "";
                    values.lastName = "";
                    values.email = "";
                    values.issue = "";
                    values.questionText = "";
                }}
            >
                {({
                    values,
                    errors,
                    touched,
                    handleChange,
                    handleBlur,
                    handleSubmit,
                    isSubmitting,
                    /* and other goodies */
                }) => (
                    <form onSubmit={handleSubmit}>
                        <div className="gform-body">
                            <div className="gform_fields">
                                <div className="gfield">
                                    <label>First Name</label>
                                    <input
                                        type="firstName"
                                        name="firstName"
                                        required
                                        onChange={handleChange}
                                        onBlur={handleBlur}
                                        value={values.firstName}
                                    />
                                    {errors.firstName && touched.firstName && errors.firstName}
                                </div>
                                <div className="gfield">
                                    <label>Last Name</label>
                                    <input
                                        type="lastName"
                                        name="lastName"
                                        required
                                        onChange={handleChange}
                                        onBlur={handleBlur}
                                        value={values.lastName}
                                    />
                                    {errors.lastName && touched.lastName && errors.lastName}
                                </div>
                                <div className="gfield">
                                    <label>Email Address</label>
                                    <input type="email" name="email" required onChange={handleChange} onBlur={handleBlur} value={values.email} />
                                    {errors.email && touched.email && errors.email}
                                </div>
                                <div className="gfield issue-options">
                                    <p>Please select an issue:</p>
                                    <label className="issue-label">
                                        <Field type="radio" name="issue" value="owe-back-taxes" />
                                        <span className="custom-bubble"></span> Owe Back Taxes
                                    </label>
                                    <label className="issue-label">
                                        <Field type="radio" name="issue" value="irs-audits" /> <span className="custom-bubble"></span> IRS Audits
                                    </label>
                                    <label className="issue-label">
                                        <Field type="radio" name="issue" value="unified-returns" /> <span className="custom-bubble"></span> Unified
                                        Returns
                                    </label>
                                    <label className="issue-label">
                                        <Field type="radio" name="issue" value="payroll-issues" /> <span className="custom-bubble"></span> Payroll
                                        Issues
                                    </label>
                                    <label className="issue-label">
                                        <Field type="radio" name="issue" value="penalties" /> <span className="custom-bubble"></span> Penalties
                                    </label>
                                    <label className="issue-label">
                                        <Field type="radio" name="issue" value="state-issues" /> <span className="custom-bubble"></span> State Issues
                                    </label>
                                    <label className="issue-label">
                                        <Field type="radio" name="issue" value="small-business-issues" />
                                        <span className="custom-bubble"></span> Small Business Issues
                                    </label>
                                    <label className="issue-label">
                                        <Field type="radio" name="issue" value="other" /> <span className="custom-bubble"></span> Other{" "}
                                    </label>
                                </div>
                            </div>
                            <div className="gfield" style={{ marginTop: "20px" }}>
                                <textarea
                                    id="questionText"
                                    style={{ width: "100%", padding: "15px", color: "grey", resize: "vertical", height: "100px" }}
                                    name="questionText"
                                    required
                                    placeholder="Enter your question here"
                                    onChange={handleChange}
                                    onBlur={handleBlur}
                                    value={values.questionText}
                                ></textarea>
                            </div>
                            <div className="gform_footer">
                                <button className="gform_button" type="submit" disabled={isSubmitting}>
                                    Get Started Now
                                </button>
                            </div>
                        </div>
                    </form>
                )}
            </Formik>
        </div>
    );
};
