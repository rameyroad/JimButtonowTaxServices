export interface ItemBody {
    id?: string;
    media?: Media;
    hasValue?: boolean;
    value?: string;
    page?: DynamicPage;
}

export interface Item {
    $type: string;
    author: QuoteBody;
    body: ItemBody;
    componentName: StringField;
    componentProps: StringField;
    aspect?: Aspect;
    id: string;
    type: string;
    cssStyles: StringField;
}

export interface Block {
    $type: string;
    author: QuoteBody;
    body?: BlockBody;
    componentName: StringField;
    componentProps: StringField;
    title: string;
    slug: StringField;
    id: string;
    type: string;
    cssStyles: StringField;
    items?: Item[];
}

export interface DynamicPage {
    regions?: Regions;
    siteId: string;
    parentId: null;
    sortOrder: number;
    navigationTitle: string;
    isHidden: boolean;
    originalPageId: null;
    isStartPage: boolean;
    showTitle: boolean;
    showExcerpt: boolean;
    slug: string;
    permaLink: string;
    metaTitle: null | string;
    metaKeywords: string;
    metaDescription: string;
    metaIndex: boolean;
    metaFollow: boolean;
    metaPriority: number;
    ogTitle: null;
    ogDescription: null;
    ogImage: Image;
    primaryImage: Image;
    author: StringField;
    authorImage: Image;
    excerpt: string;
    route: null;
    redirectUrl: null | string;
    redirectType: number;
    blocks: Block[];
    enableComments: boolean;
    closeCommentsAfterDays: number;
    commentCount: number;
    isCommentsOpen: boolean;
    published: Date;
    isPublished: boolean;
    id: string;
    typeId: string;
    title: string;
    permissions: string[];
    created: Date;
    lastModified: Date;
}

export interface DynamicPost {
    regions: Regions;
    blogId: string;
    category: Category;
    tags: string[];
    slug: string;
    permaLink: string;
    metaTitle: null;
    metaKeywords: null;
    metaDescription: null;
    metaIndex: boolean;
    metaFollow: boolean;
    metaPriority: number;
    ogTitle: null;
    ogDescription: null;
    ogImage: Image;
    primaryImage: Image;
    excerpt: null;
    route: null;
    redirectUrl: null;
    redirectType: number;
    blocks: Block[];
    enableComments: boolean;
    closeCommentsAfterDays: number;
    commentCount: number;
    isCommentsOpen: boolean;
    published: Date;
    isPublished: boolean;
    id: string;
    typeId: string;
    title: string;
    permissions: string[];
    created: Date;
    lastModified: Date;
}
export interface Category {
    id: string;
    title: string;
    slug: string;
    type: number;
}

export interface Media {
    properties: Properties;
    versions: Version[];
    id: string;
    folderId: null;
    type: number;
    filename: string;
    contentType: string;
    title: null;
    altText: null;
    description: null;
    size: number;
    publicUrl: string;
    width: number;
    height: number;
    created: Date;
    lastModified: Date;
}

export interface Properties {
    id: string;
}

export interface Version {
    id: string;
    size: number;
    width: number;
    height: number;
    fileExtension: FileExtension;
}

export enum FileExtension {
    Jpg = ".jpg",
}

export interface Aspect {
    value: number;
}

export interface BlockBody {
    media?: Media;
    value: string;
}

export interface QuoteBody {
    value: string;
}

export interface Image {
    id: null | string;
    media: Media | null;
    hasValue: boolean;
}

export interface Regions {
    $type: string;
    AuthorInfo: AuthorInfo;
}

export interface AuthorInfo {
    $type: string;
    Name: Name;
    Title: Name;
}

export interface Name {
    $type: string;
    value: string;
}

export interface StringField {
    value: string;
}
