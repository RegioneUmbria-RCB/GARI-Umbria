const { defineConfig } = require("eslint/config");
const globals = require("globals");
const tsParser = require("@typescript-eslint/parser");
const importPlugin = require("eslint-plugin-import");
const angularEslintPlugin = require("@angular-eslint/eslint-plugin");
const tsEslintPlugin = require("@typescript-eslint/eslint-plugin");
const { fixupPluginRules } = require("@eslint/compat");

// Custom rule to disallow `new Date()`
const noNewDateRule = {
  meta: {
    type: "problem",
    docs: {
      description: "Disallow `new Date()`. Use DateBuilder instead.",
    },
    schema: [],
    messages: {
      noNewDate: "Do not use `new Date()`. Use `DateBuilder` instead.",
    },
  },
  create(context) {
    return {
      NewExpression(node) {
        if (node.callee.name === "Date") {
          context.report({ node, messageId: "noNewDate" });
        }
      },
    };
  },
};

module.exports = defineConfig([
  {
    files: ["**/*.{js,mjs,cjs,ts,mts,cts}"],
    languageOptions: {
      globals: {
        ...globals.browser,
        ...globals.node,
      },
      parser: tsParser,
      parserOptions: {
        sourceType: "module",
        project: ["./tsconfig.json"],
      },
    },
    plugins: {
      import: fixupPluginRules(importPlugin),
      "@angular-eslint": fixupPluginRules(angularEslintPlugin),
      "@typescript-eslint": fixupPluginRules(tsEslintPlugin),
      "custom-rules": {
        rules: {
          "no-new-date": noNewDateRule,
        },
      },
    },
    rules: {
      // Angular rules (remove any deprecated ones)
      "@angular-eslint/component-class-suffix": "error",
      "@angular-eslint/directive-class-suffix": "error",
      "@angular-eslint/no-input-rename": "error",
      "@angular-eslint/no-inputs-metadata-property": "error",
      "@angular-eslint/no-output-on-prefix": "error",
      "@angular-eslint/no-output-rename": "error",
      "@angular-eslint/no-outputs-metadata-property": "error",
      "@angular-eslint/use-lifecycle-interface": "error",
      "@angular-eslint/use-pipe-transform-interface": "error",

      // TypeScript ESLint rules
      "@typescript-eslint/consistent-type-definitions": "error",
      "@typescript-eslint/member-ordering": "off",
      "@typescript-eslint/no-empty-interface": "error",
      "@typescript-eslint/no-inferrable-types": "off",
      "@typescript-eslint/no-misused-new": "error",
      "@typescript-eslint/no-non-null-assertion": "error",
      "@typescript-eslint/no-shadow": ["error", { "hoist": "all" }],
      "@typescript-eslint/no-unused-expressions": "error",
      "@typescript-eslint/no-unused-vars": "warn",
      "@typescript-eslint/no-use-before-define": "error",
      "@typescript-eslint/prefer-function-type": "error",
      "@typescript-eslint/unified-signatures": "error",

      // Base ESLint rules
      "arrow-body-style": "warn",
      "brace-style": ["warn", "1tbs"],
      "constructor-super": "error",
      "eol-last": "off",
      "eqeqeq": "off",
      "guard-for-in": "off",
      "import/no-deprecated": "warn",
      "no-bitwise": "error",
      "no-caller": "error",
      "no-console": [
        "warn",
        {
          allow: [
            "log",
            "warn",
            "dir",
            "timeLog",
            "assert",
            "clear",
            "count",
            "countReset",
            "group",
            "groupEnd",
            "table",
            "dirxml",
            "error",
            "groupCollapsed",
            "Console",
            "profile",
            "profileEnd",
            "timeStamp",
            "context",
          ],
        },
      ],
      "no-debugger": "error",
      "no-eval": "error",
      "no-fallthrough": "off",
      "no-new-wrappers": "error",
      "no-restricted-imports": ["error", "rxjs/Rx"],
      "no-shadow": "off",
      "no-throw-literal": "error",
      "no-trailing-spaces": "warn",
      "no-undef-init": "error",
      "no-unused-expressions": "off",
      "no-unused-labels": "error",
      "no-use-before-define": "off",
      "no-var": "error",
      "radix": "off",
      "semi": "warn",
      "no-multiple-empty-lines": ["warn", { "max": 1, "maxBOF": 0 }],

      // Custom rule usage:
      "custom-rules/no-new-date": "off",
    },
  },
]);
